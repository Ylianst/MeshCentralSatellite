/* INTEL CONFIDENTIAL
 * Copyright (C) 2011-2022 Intel Corporation.
 * This software and the related documents are Intel copyrighted materials, and your use of them is governed by the express license under which they were provided to you ("License").
 * Unless the License provides otherwise, you may not use, modify, copy, publish, distribute, disclose or transmit this software or the related documents without Intel's prior written permission.
 * This software and the related documents are provided as is, with no express or implied warranties, other than those that are expressly stated in the License.
 */

using System;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.DirectoryServices.Protocols;
using System.Net;
using SearchScope = System.DirectoryServices.SearchScope;
using System.DirectoryServices.ActiveDirectory;

namespace MeshServersCommon.code.AD
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        // https://support.microsoft.com/en-us/help/305144/how-to-use-the-useraccountcontrol-flags-to-manipulate-user-account-pro
        private const int WorkstationTrustAccountDoNotExpirePasswordFlag = 0x11000;
        private const string LdapPrefix = "LDAP://";
        private string _ldapUrl = LdapPrefix;

        public void SetLdapServerAndPort(string serverName, uint port)
        {
            _ldapUrl = $"{LdapPrefix}{serverName}:{port}/";
        }

        public string GetRootDseFromDomainName(string domainName)
        {
            using (var rootDse = new DirectoryEntry($"{LdapPrefix}{domainName}/rootDSE"))
            {
                var hostName = rootDse.Properties["dnsHostName"].Value.ToString();
                rootDse.Close();
                return hostName;
            }
        }

        public string GetDomainFromDistinguishedName(string distinguishedName)
        {
            var domainSb = new StringBuilder("");
            if (string.IsNullOrEmpty(distinguishedName))
                return string.Empty;
            var dnComponents = distinguishedName.ToLower().Split(',');
            if (!dnComponents.Any())
                return string.Empty;
            foreach (var dnComponent in dnComponents)
            {
                if (dnComponent.TrimStart().StartsWith("dc"))
                {
                    domainSb.Append(dnComponent.Split('=')[1].Trim() + ".");
                }
            }
            var domain = domainSb.ToString();
            if (!string.IsNullOrEmpty(domain))
                domain = domain.Remove(domain.Length - 1); // Remove last dot
            return domain;
        }

        public bool ObjectExists(string distinguishedName)
        {
            return DirectoryEntry.Exists(_ldapUrl + distinguishedName);
        }

        public void AddComputerToOrgUnit(ActiveDirectoryComputerObject computerObject, string orgUnit)
        {
            if (computerObject == null)
            {
                throw new ArgumentNullException(nameof(computerObject));
            }
            using (var entry = new DirectoryEntry($"{_ldapUrl}{orgUnit}"))
            using (var newComputer = entry.Children.Add($"CN={computerObject.HostName}", "computer"))
            {
                newComputer.Properties["sAMAccountName"].Value = string.IsNullOrEmpty(computerObject.SamAccountName)
                    ? computerObject.HostName + "$"
                    : computerObject.SamAccountName;
                newComputer.Properties["UserAccountControl"].Value = WorkstationTrustAccountDoNotExpirePasswordFlag;
                newComputer.Properties["dNSHostName"].Value = computerObject.DnsFqdn;
                newComputer.Properties["userPrincipalName"].Value = computerObject.UserPrincipalName;
                newComputer.CommitChanges();
                if (!string.IsNullOrWhiteSpace(computerObject.Password))
                {
                    newComputer.Invoke("SetPassword", computerObject.Password);
                }

                newComputer.Close();
                entry.Close();
            }
        }

        public void ChangeComputerPassword(string distinguishedName, string password)
        {
            using (var entry = new DirectoryEntry($"{_ldapUrl}{distinguishedName}"))
            {
                entry.Invoke("SetPassword", password);
                entry.Close();
            }
        }

        public void RemoveObject(string distinguishedName)
        {
            using (var entry = new DirectoryEntry($"{_ldapUrl}{distinguishedName}"))
            {
                entry.DeleteTree();
                entry.CommitChanges();
                entry.Close();
            }
        }

        public void AddObjectToGroup(string objectDistinguishedName, string groupDistinguishedName)
        {
            using (var groupEntry = new DirectoryEntry($"{_ldapUrl}{groupDistinguishedName}"))
            {
                groupEntry.Properties["member"].Add(objectDistinguishedName);
                groupEntry.CommitChanges();
                groupEntry.Close();
            }
        }

        public Guid GetObjectGuid(string distinguishedName)
        {
            using (var entry = new DirectoryEntry($"{_ldapUrl}{distinguishedName}"))
            {
                var objectGuid = new Guid((byte[])entry.Properties["objectGUID"].Value);
                entry.Close();
                return objectGuid;
            }
        }

        public void MapCertificateToComputer(X509Certificate2 certificate, string computerDistinguishedName)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }
            using (var entry = new DirectoryEntry($"{_ldapUrl}{computerDistinguishedName}"))
            {
                var userCertificate = entry.Properties["userCertificate"];
                var altSecIds = entry.Properties["altSecurityIdentities"];
                userCertificate.Add(certificate.RawData);
                altSecIds.Add("X509:<I>" + certificate.Issuer + "<S>" + certificate.Subject + ",SERIALNUMBER=" +
                              certificate.GetSerialNumberString());
                entry.CommitChanges();
                entry.Close();
            }
        }

        public IPAddress GetIpAddress()
        {
            string hostName = Dns.GetHostName(); // Retrieve the Name of HOST
            IPAddress[] localIpAddresses = Dns.GetHostEntry(hostName).AddressList;
            if (localIpAddresses.Length > 0)
            {
                try
                {
                    return localIpAddresses.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                catch (Exception e)
                {
                    MeshLogger.Error(e, "IPv4 not found");
                }
            }
            else
            {
                MeshLogger.Error("No IP found in system. Check network configuration");
            }

            return null;
        }

        public string GetDomainUserEmail(string userName)
        {
            try
            {
                using (var directorySearcher = GetDirectorySearcherFromCurrentForest())
                {
                    directorySearcher.Filter = $"(&(objectCategory=person)(objectClass=user)(|(sAMAccountName={userName})(userPrincipalName={userName})))";
                    var searchResult = directorySearcher.FindOne();

                    return searchResult?.Properties["userPrincipalName"][0].ToString();
                }
            }
            catch
            {
                return null;
            }
        }

        public bool ValidDomainUserByEmail(string email)
        {

            try
            {
                using (var directorySearcher = GetDirectorySearcherFromCurrentForest())
                {
                    directorySearcher.Filter = string.Format("(userprincipalname={0})", email);
                    directorySearcher.SearchScope = SearchScope.Subtree;
                    var searchResult = directorySearcher.FindOne();
                    return searchResult != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public DirectorySearcher GetDirectorySearcherFromCurrentForest()
        {
            using (var currentForest = Forest.GetCurrentForest())
            using (var directoryEntryGlobalCatalog = currentForest.FindGlobalCatalog()) {
                return directoryEntryGlobalCatalog.GetDirectorySearcher();
            }
        }

        public DirectorySearcher GetDirectorySearcherFromEntry(string domainEntry)
        {
            using (var adEntry = new DirectoryEntry(domainEntry))
                return new DirectorySearcher(adEntry);
        }

        public bool IsWindowsCredentialsValid(int globalCatalogPort, string upn, string password)
        {
            bool result;
            try
            {
                string server = string.Empty;
                int port = globalCatalogPort;

                LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(server, port);

                using (LdapConnection ldapConnection = new LdapConnection(ldapDirectoryIdentifier))
                {
                    NetworkCredential networkCredential = new NetworkCredential(upn, password);
                    ldapConnection.Credential = networkCredential;
                    ldapConnection.Bind();
                    result = true;
                }
            }
            catch (Exception e)
            {
                MeshLogger.Error(e, $"Error occurred trying to validate domain password of user {upn}");
                result = false;
            }
            return result;
        }
    }
}
