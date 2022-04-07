/* INTEL CONFIDENTIAL
 * Copyright (C) 2011-2022 Intel Corporation.
 * This software and the related documents are Intel copyrighted materials, and your use of them is governed by the express license under which they were provided to you ("License").
 * Unless the License provides otherwise, you may not use, modify, copy, publish, distribute, disclose or transmit this software or the related documents without Intel's prior written permission.
 * This software and the related documents are provided as is, with no express or implied warranties, other than those that are expressly stated in the License.
 */

using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Cryptography.X509Certificates;
using MeshServersCommon.code.DB.Managers;

namespace MeshServersCommon.code.AD
{
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

    public class CertificateAuthorityInfo
    {
        private readonly IServerSettingManager _serverSettingManager;

        public CertificateAuthorityInfo(IServerSettingManager serverSettingManager)
        {
            _serverSettingManager = serverSettingManager;
        }

        public List<CertificateAuthority> GetCertificateAuthoritiesTemplatesAndTrustedRoots()
        {
            CertificateAuthority[] CAs = null;
            List<CertificateAuthority> casList = new List<CertificateAuthority>();
            List<X509Certificate2> trustedRoots = new List<X509Certificate2>();
            var ldapConnectionPort =
                int.Parse(_serverSettingManager.GetSettingKeyValue("WebServer", "LdapConnectionPort"));

            try
            {
                var activeDirectoryService = new ActiveDirectoryService();
                using (var forestSearcher = activeDirectoryService.GetDirectorySearcherFromCurrentForest())
                {
                    var domainResults = forestSearcher.FindAll();

                    foreach (SearchResult domain in domainResults)
                    {
                        var domainDistinguishedName = domain.Properties["distinguishedName"][0].ToString();
                        var domainFqdn = activeDirectoryService.GetDomainFromDistinguishedName(domainDistinguishedName);


                        if (string.IsNullOrWhiteSpace(domainDistinguishedName))
                            continue;
                        try
                        {

                            string domainEntry =
                                $"LDAP://{domainFqdn}:{ldapConnectionPort}/CN=Enrollment Services,CN=Public Key Services,CN=Services,CN=Configuration,{domainDistinguishedName}";
                            // Create an AD entry to search on
                            using (var dsearch = activeDirectoryService.GetDirectorySearcherFromEntry(domainEntry))
                            {
                                dsearch.Filter = "(objectCategory=pKIEnrollmentService)";
                                dsearch.PropertiesToLoad.Add("Name");
                                dsearch.PropertiesToLoad.Add("dNSHostName");
                                dsearch.PropertiesToLoad.Add("certificateTemplates");
                                dsearch.PropertiesToLoad.Add("cACertificate");
                                var res = dsearch.FindAll();
                                if (null == res)
                                    return null;

                                var domainEntry2 =
                                    $"LDAP://{domainFqdn}:{ldapConnectionPort}/CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,{domainDistinguishedName}";
                                // Create an AD entry to search on
                                using (var dsearch2 = activeDirectoryService.GetDirectorySearcherFromEntry(domainEntry2))
                                {
                                    dsearch2.Filter = "(objectCategory=pKICertificateTemplate)";
                                    dsearch2.PropertiesToLoad.Add("Name");
                                    var res2 = dsearch2.FindAll();
                                    if (null == res2)
                                        return null;

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
                                            for (int i = 0; i < coll.Count; i++)
                                            {
                                                CAs[index].Templates.Add(coll[i].ToString());
                                            }
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

                        if (CAs == null)
                            continue;

                        foreach (CertificateAuthority ca in CAs)
                        {
                            if (!casList.Contains(ca))
                            {
                                casList.Add(ca);
                            }
                        }
                    }
                }
                MarkRootCAs(casList, trustedRoots);
            }
            catch { } // Do not remove this catch, it's expected to swallow any error. We need to revisit the errors and fix

            return casList;
        }

        private string GetIssuerName(X509Certificate2 cert)
        {
            var issuerComponents = cert.Issuer.Split(',');
            foreach (string comp in issuerComponents)
            {
                if (comp.Trim().ToUpper().StartsWith("CN="))
                {
                    return comp.Split('=')[1].Trim();
                }
            }

            return null;
        }

        private void MarkRootCAs(List<CertificateAuthority> certificateAuthorities, List<X509Certificate2> rootCertificates)
        {
            if (certificateAuthorities == null || certificateAuthorities.Count == 0 ||
                rootCertificates == null || rootCertificates.Count == 0)
            {
                return;
            }

            foreach (CertificateAuthority ca in certificateAuthorities)
            {
                foreach (X509Certificate2 cer in rootCertificates)
                {
                    if (GetIssuerName(cer) == ca.CAName)
                    {
                        ca.IsRoot = true;
                        break;
                    }
                }
            }
        }
    }
}
