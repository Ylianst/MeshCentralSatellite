/*
Copyright 2022 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace MeshCentralSatellite
{
    public class DomainControllerServices
    {
        private const int WorkstationTrustAccountDoNotExpirePasswordFlag = 0x11000;
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static Regex amtPasswordRgx = new Regex(@"^(?=.{20}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[+\/]).*$");
        private string rootDistinguishedName;
        private string domain;
        private string ldapUrl = "LDAP://RootDSE/";

        // This is from https://docs.microsoft.com/en-us/windows/win32/api/certadm/nf-certadm-icertadmin-revokecertificate.
        public enum CertRevokeReason
        {
            CrlReasonUnspecified = 0,
            CrlReasonKeyCompromise = 1,
            CrlReasonCaCompromise = 2,
            CrlReasonAffiliationChanged = 3,
            CrlReasonSuperseded = 4,
            CrlReasonCessationOfOperation = 5,
            CrlReasonCertificateHold = 6
        }

        public class ActiveDirectoryComputerObject
        {
            public string HostName { get; set; }
            public string SamAccountName { get; set; }
            public string DnsFqdn { get; set; }
            public string UserPrincipalName { get; set; }
            public string DistinguishedName { get; set; }
            public Guid Uuid { get; set; }
            public string Password { get; set; }
            public bool AlreadyPresent { get; set; }
        }

        /// <summary>
        /// Used to communicate with Intel AMT devices
        /// Obtained from the Intel AMT Implementation and Reference Guide at https://software.intel.com/sites/manageability/AMT_Implementation_and_Reference_Guide/default.htm
        /// </summary>
        public struct ManageabilityPorts
        {
            public const int
                AMT_HTTP = 16992,
                AMT_HTTPS = 16993,
                AMT_Redirection_Tcp = 16994,
                AMT_Redirection_Tls = 16995,
                ASF_RMCP = 623,
                ASF_RMCP_Secure = 664,
                VNC_KVM = 5900;
        }

        public class LdapObjectRef
        {
            public const string OrganizationUnit = "OU";
            public const string UserOrComputer = "CN";
            public const string DomainComponent = "DC";
            public const string SchemaNameUsers = "user";
            public const string SchemaNameComputers = "computer";

            /// <summary>
            /// Used in AD objects and client certificates for 802.1X
            /// Complete form is
            ///     HTTP/hostname.domain:port
            /// </summary>
            public struct AmtServicePrincipalNames
            {
                public static readonly string Rmcp = $":{ManageabilityPorts.ASF_RMCP}";
                public static readonly string SecureRmcp = $":{ManageabilityPorts.ASF_RMCP_Secure}";
                public static readonly string Http = $":{ManageabilityPorts.AMT_HTTP}";
                public static readonly string Https = $":{ManageabilityPorts.AMT_HTTPS}";
                public static readonly string TcpRedirect = $":{ManageabilityPorts.AMT_Redirection_Tcp}";
                public static readonly string TlsRedirect = $":{ManageabilityPorts.AMT_Redirection_Tls}";
                public static readonly string VncKvm = $":{ManageabilityPorts.VNC_KVM}";
            }
        }

        public string DomainName { get { return domain; } }

        public static bool isComputerJoinedToDomain()
        {
            Domain d = null;
            try { d = Domain.GetComputerDomain(); } catch (Exception) { }
            return (d != null);
        }

        public DomainControllerServices()
        {
            DirectoryEntry RootDirEntry = new DirectoryEntry("LDAP://RootDSE");
            rootDistinguishedName = RootDirEntry.Properties["defaultNamingContext"].Value.ToString();
            domain = GetDomainFromDistinguishedName(rootDistinguishedName);
            DirectoryContext domainContext = new DirectoryContext(DirectoryContextType.Domain, domain);
            DomainController controller = Domain.GetDomain(domainContext).FindDomainController();
            ldapUrl = "LDAP://" + controller.Name + "/";
        }

        private static string GetDomainFromDistinguishedName(string distinguishedName)
        {
            var domainSb = new StringBuilder("");
            if (string.IsNullOrEmpty(distinguishedName)) return string.Empty;
            var dnComponents = distinguishedName.ToLower().Split(',');
            if (!dnComponents.Any()) return string.Empty;
            foreach (var dnComponent in dnComponents)
            {
                if (dnComponent.TrimStart().StartsWith("dc")) { domainSb.Append(dnComponent.Split('=')[1].Trim() + "."); }
            }
            var domain = domainSb.ToString();
            if (!string.IsNullOrEmpty(domain)) { domain = domain.Remove(domain.Length - 1); } // Remove last dot
            return domain;
        }

        public static string GetFirstCommonNameFromDistinguishedName(string distinguishedName)
        {
            string[] dnComponents = distinguishedName.Split(',');
            foreach (var dnComponent in dnComponents) { if (dnComponent.TrimStart().StartsWith("CN=")) { return dnComponent.TrimStart().Substring(3); } }
            return null;
        }

        private ActiveDirectoryComputerObject CreateComputerObject(string computerIdentifier)
        {
            // My domain controller does not have organization units... but if I did, how would I get this information?
            string orgUnitDistinguishedName = "CN=Computers," + rootDistinguishedName;

            // Create the computer object
            ActiveDirectoryComputerObject computer = new ActiveDirectoryComputerObject
            {
                HostName = $"iME-{computerIdentifier}",
                UserPrincipalName = $"iME-{computerIdentifier}@{domain}",
                DnsFqdn = $"iME-{computerIdentifier}.{domain}",
                SamAccountName = $"iME${computerIdentifier}$",
                DistinguishedName = $"CN=iME-{computerIdentifier},{orgUnitDistinguishedName}",
                AlreadyPresent = false
            };
            return computer;
        }

        public List<string> getSecurityGroups()
        {
            // TODO: Add org unit support?
            // TODO: Right now, looking for security groups in the "Computers" section.
            string orgUnitDistinguishedName = "CN=Computers," + rootDistinguishedName;

            List<string> groups = new List<string>();
            using (var root = new DirectoryEntry(ldapUrl + orgUnitDistinguishedName))
            {
                using (var srch = new DirectorySearcher(root, "(&(objectCategory=group)(groupType:1.2.840.113556.1.4.803:=2147483648))"))
                {
                    SearchResultCollection results = srch.FindAll();
                    if (results != null)
                    {
                        foreach (SearchResult result in results) { groups.Add((string)result.Properties["DistinguishedName"][0]);}
                    }
                }
            }
            return groups;
        }

        public ActiveDirectoryComputerObject CreateComputer(string computerIdentifier, string description, List<string> securityGroupDistinguishedNames)
        {
            // TODO: Add org unit support?
            string orgUnitDistinguishedName = "CN=Computers," + rootDistinguishedName;

            // Create the computer object
            ActiveDirectoryComputerObject computer = CreateComputerObject(computerIdentifier);
            computer.Password = GeneratePassword(); // Randomize a new password

            // Add the computer to the active directory
            if (!DirectoryEntry.Exists(ldapUrl + computer.DistinguishedName))
            {
                using (var entry = new DirectoryEntry(ldapUrl + orgUnitDistinguishedName))
                {
                    using (var newComputer = entry.Children.Add($"CN={computer.HostName}", "computer"))
                    {
                        newComputer.Properties["sAMAccountName"].Value = string.IsNullOrEmpty(computer.SamAccountName)
                            ? computer.HostName + "$"
                            : computer.SamAccountName;
                        newComputer.Properties["UserAccountControl"].Value = WorkstationTrustAccountDoNotExpirePasswordFlag;
                        newComputer.Properties["dNSHostName"].Value = computer.DnsFqdn;
                        newComputer.Properties["userPrincipalName"].Value = computer.UserPrincipalName;
                        if (description != null) { newComputer.Properties["Description"].Value = description; }
                        newComputer.CommitChanges();
                    }
                }
            }
            else
            {
                computer.AlreadyPresent = true;
            }

            // Set or reset the computer password
            using (var entry = new DirectoryEntry(ldapUrl + computer.DistinguishedName))
            {
                entry.Invoke("SetPassword", computer.Password);
            }

            // Get the object GUID
            using (DirectoryEntry entry = new DirectoryEntry(ldapUrl + computer.DistinguishedName))
            {
                computer.Uuid = new Guid((byte[])entry.Properties["objectGUID"].Value);
            }

            // Add computer to security groups if needed
            if (securityGroupDistinguishedNames != null)
            {
                foreach (string securityGroupDistinguishedName in securityGroupDistinguishedNames.Where(securityGroupDistinguishedName => !string.IsNullOrEmpty(securityGroupDistinguishedName)))
                {
                    if (DirectoryEntry.Exists(ldapUrl + securityGroupDistinguishedName))
                    {
                        using (var groupEntry = new DirectoryEntry(ldapUrl + securityGroupDistinguishedName))
                        {
                            groupEntry.Properties["member"].Add(computer.DistinguishedName);
                            groupEntry.CommitChanges();
                        }
                    }
                }
            }

            // TODO: Do work to get a certificate if needed

            return computer;
        }

        public bool RemoveComputer(string computerIdentifier)
        {
            // My domain controller does not have organization units... but if I did, how would I get this information?
            string orgUnitDistinguishedName = "CN=Computers," + rootDistinguishedName;

            // Create the computer distinguished name
            string DistinguishedName = $"CN=iME-{computerIdentifier},{orgUnitDistinguishedName}";

            // Remove the computer from the active directory
            if (DirectoryEntry.Exists(ldapUrl + DistinguishedName))
            {
                // TODO: Revoke certificate if needed

                // Remove the computer entry from the active directory
                using (var entry = new DirectoryEntry(ldapUrl + DistinguishedName))
                {
                    entry.DeleteTree();
                    return true;
                }
            }
            return false;
        }

        // Debug code, list objects in a part of the active directory
        public string[] ListDomainObjects()
        {
            List<string> list = new List<string>();
            using (var entry = new DirectoryEntry(ldapUrl + rootDistinguishedName))
            {
                foreach (DirectoryEntry child in entry.Children)
                {
                    list.Add(child.Path);
                }
            }
            return (string[])list.ToArray();
        }

        // Generate a string Intel AMT password
        private static string GeneratePassword()
        {
            byte[] randomBytes = new byte[20];
            string randomString;
            do {
                rng.GetBytes(randomBytes);
                randomString = Convert.ToBase64String(randomBytes, Base64FormattingOptions.None).Substring(0, 20);
            } while (!amtPasswordRgx.IsMatch(randomString));
            return randomString;
        }


        public static DirectorySearcher GetDirectorySearcherFromCurrentForest()
        {
            using (var currentForest = Forest.GetCurrentForest())
            using (var directoryEntryGlobalCatalog = currentForest.FindGlobalCatalog())
            {
                return directoryEntryGlobalCatalog.GetDirectorySearcher();
            }
        }

        public static DirectorySearcher GetDirectorySearcherFromEntry(string domainEntry)
        {
            using (var adEntry = new DirectoryEntry(domainEntry))
                return new DirectorySearcher(adEntry);
        }


        public class CertificateAuthority
        {
            public CertificateAuthority()
            {
                Templates = new List<string>();
                RootCertificate = new X509Certificate2();
            }
            public string DNSName { get; set; }
            public string CAName { get; set; }
            public List<string> Templates { get; set; }
            public X509Certificate2 RootCertificate { get; set; }
            public bool IsRoot { get; set; }
        }

        public List<CertificateAuthority> GetCertificateAuthoritiesTemplatesAndTrustedRoots()
        {
            CertificateAuthority[] CAs = null;
            List<CertificateAuthority> casList = new List<CertificateAuthority>();
            List<X509Certificate2> trustedRoots = new List<X509Certificate2>();

            try
            {
                using (var forestSearcher = GetDirectorySearcherFromCurrentForest())
                {
                    var domainResults = forestSearcher.FindAll();

                    foreach (SearchResult domain in domainResults)
                    {
                        var domainDistinguishedName = domain.Properties["distinguishedName"][0].ToString();
                        var domainFqdn = GetDomainFromDistinguishedName(domainDistinguishedName);

                        if (string.IsNullOrWhiteSpace(domainDistinguishedName)) continue;
                        try
                        {

                            string domainEntry = ldapUrl + "CN=Enrollment Services,CN=Public Key Services,CN=Services,CN=Configuration," + domainDistinguishedName;
                            // Create an AD entry to search on
                            using (var dsearch = GetDirectorySearcherFromEntry(domainEntry))
                            {
                                dsearch.Filter = "(objectCategory=pKIEnrollmentService)";
                                dsearch.PropertiesToLoad.Add("Name");
                                dsearch.PropertiesToLoad.Add("dNSHostName");
                                dsearch.PropertiesToLoad.Add("certificateTemplates");
                                dsearch.PropertiesToLoad.Add("cACertificate");
                                var res = dsearch.FindAll();
                                if (null == res) return null;

                                var domainEntry2 = ldapUrl + "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration," + domainDistinguishedName;
                                // Create an AD entry to search on
                                using (var dsearch2 = GetDirectorySearcherFromEntry(domainEntry2))
                                {
                                    dsearch2.Filter = "(objectCategory=pKICertificateTemplate)";
                                    dsearch2.PropertiesToLoad.Add("Name");
                                    var res2 = dsearch2.FindAll();
                                    if (null == res2) return null;

                                    CAs = new CertificateAuthority[res.Count];
                                    int index = 0;
                                    foreach (SearchResult result in res)
                                    {
                                        CAs[index] = new CertificateAuthority();
                                        ResultPropertyValueCollection coll = result.Properties["Name"];
                                        CAs[index].CAName = coll[0].ToString();
                                        coll = result.Properties["dNSHostName"];
                                        CAs[index].DNSName = coll[0].ToString();
                                        CAs[index].IsRoot = false; // At this point we can't tell if is root or not
                                        coll = result.Properties["certificateTemplates"];
                                        if (coll != null)
                                        {
                                            for (int i = 0; i < coll.Count; i++) { CAs[index].Templates.Add(coll[i].ToString()); }
                                        }

                                        coll = result.Properties["cACertificate"];
                                        byte[] caBinaryCert = null;
                                        object binaryCert = coll[0];
                                        if (binaryCert is byte[])
                                        {
                                            caBinaryCert = (byte[])coll[0];
                                        }

                                        trustedRoots.Add(new X509Certificate2(caBinaryCert));
                                        index++;
                                    }
                                }
                            }
                        }
                        catch { } // Do not remove this catch, it's expected to swallow any error. We need to revisit the errors and fix

                        if (CAs == null) continue;

                        foreach (CertificateAuthority ca in CAs) { if (!casList.Contains(ca)) { casList.Add(ca); } }
                    }
                }
                MarkRootCAs(casList, trustedRoots);
            }
            catch { } // Do not remove this catch, it's expected to swallow any error. We need to revisit the errors and fix

            return casList;
        }

        private void MarkRootCAs(List<CertificateAuthority> certificateAuthorities, List<X509Certificate2> rootCertificates)
        {
            if (certificateAuthorities == null || certificateAuthorities.Count == 0 || rootCertificates == null || rootCertificates.Count == 0) { return; }

            foreach (CertificateAuthority ca in certificateAuthorities)
            {
                foreach (X509Certificate2 cer in rootCertificates)
                {
                    if (GetIssuerName(cer) == ca.CAName) { ca.IsRoot = true; break; }
                }
            }
        }

        private string GetIssuerName(X509Certificate2 cert)
        {
            var issuerComponents = cert.Issuer.Split(',');
            foreach (string comp in issuerComponents)
            {
                if (comp.Trim().ToUpper().StartsWith("CN=")) { return comp.Split('=')[1].Trim(); }
            }
            return null;
        }


        public enum CommonNameTypes
        {
            DNSFQDN,
            Hostname,
            SAMAccountName,
            UUID,
            UserPrincipalName,
            DistinguishedName,
            AMTRMCP,
            AMTSecureRMCP,
            AMTHTTP,
            AMTHTTPS,
            AMTTCPRedirect,
            AMTTLSRedirect,
            AMTVNCKVM
        }

        /// <summary>
        /// AuthenticationProtocol shall indicate the desired EAP (Extensible Authentication Protocol) type.
        ///* EAP-TLS(0) : shall indicate that the desired EAP type is the Transport Layer Security EAP type specified in RFC 2716. If AuthenticationProtocol contains 0, Username should not be null, ServerCertificateName and ServerCertificateNameComparison may be null or not null, and RoamingIdentity, Password, Domain, ProtectedAccessCredential, PACPassword, and PSK should be null.
        ///* EAP-TTLS/MSCHAPv2(1) : shall indicate that the desired EAP type is the Tunneled TLS Authentication Protocol EAP type specified in draft-ietf-pppext-eap-ttls, with Microsoft PPP CHAP Extensions, Version 2 (MSCHAPv2) as the inner authentication method.If AuthenticationProtocol contains 1, Username and Password should not be null, RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, and Domain may be null or not null, and ProtectedAccessCredential, PACPassword, and PSK should be null.
        ///* PEAPv0/EAP-MSCHAPv2 (2): shall indicate that the desired EAP type is the Protected Extensible Authentication Protocol(PEAP) Version 0 EAP type specified in draft-kamath-pppext-peapv0, with Microsoft PPP CHAP Extensions, Version 2 (MSCHAPv2) as the inner authentication method.If AuthenticationProtocol contains2, Username and Password should not be null, RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, and Domain may be null or not null, and ProtectedAccessCredential, PACPassword, and PSK should be null.
        ///* PEAPv1/EAP-GTC (3): shall indicate that the desired EAP type is the Protected Extensible Authentication Protocol(PEAP) Version 1 EAP type specified in draft-josefsson-pppext-eap-tls-eap, with Generic Token Card(GTC) as the inner authentication method.If AuthenticationProtocol contains 3, Username and Password should not be null, RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, and Domain may be null or not null, and ProtectedAccessCredential, PACPassword, and PSK should be null.
        ///* EAP-FAST/MSCHAPv2 (4): shall indicate that the desired EAP type is the Flexible Authentication Extensible Authentication Protocol EAP type specified in IETF RFC 4851, with Microsoft PPP CHAP Extensions, Version 2 (MSCHAPv2) as the inner authentication method.If AuthenticationProtocol contains 4, Username and Password should not be null, RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, Domain, ProtectedAccessCredential, and PACPassword may be null or not null, and PSK should be null.
        ///* EAP-FAST/GTC (5): shall indicate that the desired EAP type is the Flexible Authentication Extensible Authentication Protocol EAP type specified in IETF RFC 4851, with Generic Token Card(GTC) as the inner authentication method.If AuthenticationProtocol contains 5, Username and Password should not be null, RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, Domain, ProtectedAccessCredential, and PACPassword may be null or not null, and PSK should be null.
        ///* EAP-MD5 (6): shall indicate that the desired EAP type is the EAP MD5 authentication method, specified in RFC 3748. If AuthenticationProtocol contains 6, Username and Password should not be null, Domain may be null or not null, and RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, ProtectedAccessCredential, PACPassword, and PSK should be null.
        ///* EAP-PSK(7) : shall indicate that the desired EAP type is the EAP-PSK(Pre-Shared Key) EAP type specified in RFC 4764. If AuthenticationProtocol contains 7, Username and PSK should not be null, Domain and RoamingIdentity may be null or not null, and Password, ServerCertificateName, ServerCertificateNameComparison, ProtectedAccessCredential, and PACPassword should be null.
        ///* EAP-SIM(8) : shall indicate that the desired EAP type is the Extensible Authentication Protocol Method for Global System for Mobile Communications(GSM) Subscriber Identity Modules(EAP-SIM), specified in RFC 4186. If AuthenticationProtocol contains 8, Username and PSK should not be null, Domain and RoamingIdentity may be null or not null, and Password, ServerCertificateName, ServerCertificateNameComparison, ProtectedAccessCredential, and PACPassword should be null.
        ///* EAP-AKA(9) : shall indicate that the desired EAP type is the EAP Method for 3rd Generation Authentication and Key Agreement(EAP-AKA), specified in RFC 4187. If AuthenticationProtocol contains 9, Username and PSK should not be null, Domain and RoamingIdentity may be null or not null, and Password, ServerCertificateName, ServerCertificateNameComparison, ProtectedAccessCredential, and PACPassword should be null.
        ///* EAP-FAST/TLS(10) : shall indicate that the desired EAP type is the Flexible Authentication EAP type specified in IETF RFC 4851, with TLS as the inner authentication method.If AuthenticationProtocol contains 10, Username and Password should not be null, RoamingIdentity, ServerCertificateName, ServerCertificateNameComparison, Domain, ProtectedAccessCredential, and PACPassword may be null or not null, and PSK should be null.
        /// </summary>
        public enum _8021_Protocol : ushort
        {
            EAP_TLS = 0,
            EAP_TTLS_MSCHAP_V2,
            EAP_PEAP_MSCHAP_V2,
            EAP_GTC,
            EAP_FAST_MSCHAP_V2,
            EAP_FAST_GTC,
            EAP_FAST_TLS = 10
        }

        /// <summary>
        /// For protocols that support it, allows specifying how to obtain the certificate used to
        /// authenticate to the server. Required for EAP_TLS and EAP_FAST_TLS protocols.
        /// </summary>
        public enum CertificateSource
        {
            None,
            RequestFromCA,
            PickFromDatabase
        }

        public class ClientCertificateProfile
        {
            public CertificateSource Source { get; set; }
            public string Path { get; set; }
            public string Template { get; set; }
            public HashSet<CommonNameTypes> CommonNames { get; set; }
            public CommonNamesSource CommonNamesSource { get; set; }
            public CommonNameTypes DesignatedSubject { get; set; }
        }

        public class RootCertificateProfile
        {
            public CertificateSource Source { get; set; }
            public string Path { get; set; }
        }

        public enum CommonNamesSource
        {
            /// <summary>
            /// When selected, the client certificate is configured as follows:
            /// Subject - DNS Host Name (FQDN)
            /// Subject Alternate Name
            ///     DNS Host Name (FQDN)
            ///     Host Name
            ///     SAM Account Name
            ///     User Principal Name
            ///     UUID of the new AD object representing the AMT system
            /// </summary>
            Default,
            UserDefined
        }

        /*
        /// <summary>
        /// Gets Client certificate from the Certification Authority.
        /// </summary>
        /// <param name="caName">Certification Authority name</param>
        /// <param name="amtComputer">Active Directory computer</param>
        /// <param name="certProfile">Certificate profile</param>
        /// <returns>Client certificate from Certification Authority</returns>
        private X509Certificate2 GetClientCertificateFromCa(string caName, ActiveDirectoryComputerObject amtComputer, ClientCertificateProfile certProfile)
        {
            var certFields = CreateCertFields(amtComputer, certProfile);
            var keyTuple = _amt8021XManager.CreateKeyPair();
            var signedCertRequest = _amt8021XManager.GetCertSignedRequest(certProfile, amtComputer, certFields, keyTuple);
            var certificate = CertificationAuthorityService.GetClientCertFromCsr(signedCertRequest, caName);
            return certificate;
        }
        */

        /// <summary>
        /// Creates alternative name fields to be added to a certificate.
        /// </summary>
        /// <param name="amtComputer">Active Directory computer</param>
        /// <param name="certProfile">Certificate profile</param>
        /// <returns>Certificate name fields</returns>
        private Dictionary<CommonNameTypes, string> CreateCertFields(ActiveDirectoryComputerObject amtComputer, ClientCertificateProfile certProfile)
        {
            var certFields = new Dictionary<CommonNameTypes, string>();
            if (certProfile.CommonNamesSource == CommonNamesSource.Default)
            {
                certProfile.CommonNames = new HashSet<CommonNameTypes>
                {
                    CommonNameTypes.UserPrincipalName,
                    CommonNameTypes.DNSFQDN,
                    CommonNameTypes.Hostname,
                    CommonNameTypes.SAMAccountName,
                    CommonNameTypes.UUID,
                    CommonNameTypes.DistinguishedName
                };
            }

            if (certProfile.CommonNames.Contains(CommonNameTypes.UserPrincipalName))
                certFields.Add(CommonNameTypes.UserPrincipalName, amtComputer.UserPrincipalName);
            if (certProfile.CommonNames.Contains(CommonNameTypes.DNSFQDN))
                certFields.Add(CommonNameTypes.DNSFQDN, amtComputer.DnsFqdn);
            if (certProfile.CommonNames.Contains(CommonNameTypes.Hostname))
                certFields.Add(CommonNameTypes.Hostname, amtComputer.HostName);
            if (certProfile.CommonNames.Contains(CommonNameTypes.SAMAccountName))
                certFields.Add(CommonNameTypes.SAMAccountName, amtComputer.SamAccountName);
            if (certProfile.CommonNames.Contains(CommonNameTypes.UUID) && !amtComputer.Uuid.Equals(Guid.Empty))
                certFields.Add(CommonNameTypes.UUID, amtComputer.Uuid.ToString());
            if (certProfile.CommonNames.Contains(CommonNameTypes.DistinguishedName))
                certFields.Add(CommonNameTypes.DistinguishedName, amtComputer.DistinguishedName);
            const string amtPortPrefix = "HTTP/";
            certFields.Add(CommonNameTypes.AMTHTTP,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.Http}");
            certFields.Add(CommonNameTypes.AMTHTTPS,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.Https}");
            certFields.Add(CommonNameTypes.AMTRMCP,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.Rmcp}");
            certFields.Add(CommonNameTypes.AMTSecureRMCP,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.SecureRmcp}");
            certFields.Add(CommonNameTypes.AMTTCPRedirect,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.TcpRedirect}");
            certFields.Add(CommonNameTypes.AMTTLSRedirect,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.TlsRedirect}");
            certFields.Add(CommonNameTypes.AMTVNCKVM,
                $"{amtPortPrefix}{amtComputer.DnsFqdn}{LdapObjectRef.AmtServicePrincipalNames.VncKvm}");
            return certFields;
        }

    }
}
