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
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Interop.CERTADMLib;
using CERTCLILib;
using CERTENROLLLib;

namespace MeshCentralSatellite
{
    public class CertificationAuthorityService
    {
        /// <summary>
        /// BASE64 format with begin/end flag
        /// </summary>
        private const int CrOutBase64Header = 0x0;
        /// <summary>
        /// PKCS #10 request flag
        /// </summary>
        private const int CrInPkcs10 = 0x100;
        /// <summary>
        /// Unicode BASE64 format without begin/end flag
        /// </summary>
        private const int CrInBase64 = 0x1;
        private const int False = 0;

        private enum SubmitDispositionValues
        {
            CR_DISP_ERROR = 0x00000001, // Request failed
            CR_DISP_DENIED = 0x00000002, // Request denied
            CR_DISP_ISSUED = 0x00000003, // Certificate issued
            CR_DISP_UNDER_SUBMISSION = 0x00000005, // Request pending
            CR_DISP_REVOKED = 0x00000006 // Certificate revoked
        }

        public static X509Certificate2 GetRootCertificate(string caName)
        {
            ICertRequest2 certRequest2 = new CCertRequest();
            var rootCert = certRequest2.GetCACertificate(False, caName, CrOutBase64Header);
            return new X509Certificate2(Encoding.UTF8.GetBytes(rootCert));
        }

        public static X509Certificate2 GetClientCertFromCsr(string csr, string caName)
        {
            ICertRequest2 certRequest2 = new CCertRequest();
            SubmitDispositionValues disposition = (SubmitDispositionValues)certRequest2.Submit(CrInBase64 | CrInPkcs10, csr, null, caName);

            if (disposition != SubmitDispositionValues.CR_DISP_ISSUED) return null;
            var certString = certRequest2.GetCertificate(CrOutBase64Header);

            return new X509Certificate2(Encoding.UTF8.GetBytes(certString));
        }

        public static void RevokeCertificateFromCa(string caName, string serialNum, DomainControllerServices.CertRevokeReason reason)
        {
            ICertAdmin certAdmin = new CCertAdmin();
            certAdmin.RevokeCertificate(caName, serialNum, (int)reason, DateTime.UtcNow);
        }

        public static string GetCommonNameFromSubject(string subject)
        {
            string[] splitStr = subject.Split(',');
            foreach (string s in splitStr) { if (s.Trim().StartsWith("CN=")) { return s.Trim().Substring(3); } }
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

        public static string GenerateNullCsr(string distinguishedName, Dictionary<CommonNameTypes, string> extras, string templateName, CommonNameTypes designatedSubject, byte[] publicKey)
        {
            IX509CertificateRequestPkcs10 objPkcs10 = (IX509CertificateRequestPkcs10)Activator.CreateInstance(Type.GetTypeFromProgID("X509Enrollment.CX509CertificateRequestPkcs10"));

            try
            {
                var requestPkcs10 = (IX509CertificateRequestPkcs10)Activator.CreateInstance(Type.GetTypeFromProgID("X509Enrollment.CX509CertificateRequestPkcs10"));
                var publicKeyObject = GetKeyObjectFromPublicKey(publicKey);
                requestPkcs10.InitializeFromPublicKey(X509CertificateEnrollmentContext.ContextUser, publicKeyObject, templateName);
                var innerRequestPkcs10 = (IX509CertificateRequestPkcs10)requestPkcs10.GetInnerRequest(InnerRequestLevel.LevelInnermost);

                innerRequestPkcs10.Subject = GetX500DistinguishedName(designatedSubject, distinguishedName, extras);

                var objAlternativeNames = new CAlternativeNames();
                AddCommonAlternativeNames(extras, objAlternativeNames);
                AddSpnAlternativeNames(extras, objAlternativeNames);

                var extensionAlternativeNames = new CX509ExtensionAlternativeNames();
                extensionAlternativeNames.InitializeEncode(objAlternativeNames);

                // ReSharper disable once SuspiciousTypeConversion.Global
                innerRequestPkcs10.X509Extensions.Add((CX509Extension)extensionAlternativeNames);
                innerRequestPkcs10.Encode();

                return innerRequestPkcs10.RawData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get a public key object using the public key bytes, handles if the key contains key info or not
        /// </summary>
        /// <param name="publicKey">Public key bytes</param>
        /// <returns>Public key object</returns>
        private static CX509PublicKey GetKeyObjectFromPublicKey(byte[] publicKey)
        {
            var publicKeyValue = new CX509PublicKey();
            var strEncodedKey = Convert.ToBase64String(publicKey);
            try
            {
                publicKeyValue.InitializeFromEncodedPublicKeyInfo(strEncodedKey);
            }
            catch (Exception)
            {
                var keyInfo = new CObjectId();
                keyInfo.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_PUBKEY_ALG_OID_GROUP_ID,
                    ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY, AlgorithmFlags.AlgorithmFlagsNone, "RSA");
                publicKeyValue.Initialize(keyInfo, strEncodedKey, null);
            }

            return publicKeyValue;
        }

        /// <summary>
        /// Add common names contained to an CAlternativeNames object
        /// </summary>
        /// <param name="commonNames">Common names to be added</param>
        /// <param name="objAlternativeNames">Target object</param>
        private static void AddCommonAlternativeNames(Dictionary<CommonNameTypes, string> commonNames, CAlternativeNames objAlternativeNames)
        {
            if (commonNames == null) return;
            if (commonNames.ContainsKey(CommonNameTypes.DistinguishedName))
            {
                var alternativeName = new CAlternativeName();
                alternativeName.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_URL, $"LDAP://{commonNames[CommonNameTypes.DistinguishedName]}");
                objAlternativeNames.Add(alternativeName);
            }
            if (commonNames.ContainsKey(CommonNameTypes.DNSFQDN))
            {
                var dnsFqdn = new CAlternativeName();
                dnsFqdn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME, commonNames[CommonNameTypes.DNSFQDN]);
                objAlternativeNames.Add(dnsFqdn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.Hostname))
            {
                var hostname = new CAlternativeName();
                hostname.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME, commonNames[CommonNameTypes.Hostname]);
                objAlternativeNames.Add(hostname);
            }
            if (commonNames.ContainsKey(CommonNameTypes.SAMAccountName))
            {
                var samAccountName = new CAlternativeName();
                samAccountName.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME,
                    commonNames[CommonNameTypes.SAMAccountName]);
                objAlternativeNames.Add(samAccountName);
            }
            if (commonNames.ContainsKey(CommonNameTypes.UUID))
            {
                var uuid = new CAlternativeName();
                uuid.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME, commonNames[CommonNameTypes.UUID]);
                objAlternativeNames.Add(uuid);
            }
            if (commonNames.ContainsKey(CommonNameTypes.UserPrincipalName))
            {
                var userPrincipalName = new CAlternativeName();
                userPrincipalName.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME,
                    commonNames[CommonNameTypes.UserPrincipalName]);
                objAlternativeNames.Add(userPrincipalName);
            }
        }

        /// <summary>
        /// Get distinguished name from a designated subject type
        /// </summary>
        /// <param name="designatedSubject">Designated subject type</param>
        /// <param name="distinguishedName">Distinguished name string</param>
        /// <param name="commonNames">Provided common names</param>
        /// <returns></returns>
        private static CX500DistinguishedName GetX500DistinguishedName(CommonNameTypes designatedSubject,
            string distinguishedName, Dictionary<CommonNameTypes, string> commonNames)
        {
            var objDistinguishedName = new CX500DistinguishedName();

            switch (designatedSubject)
            {
                case CommonNameTypes.DistinguishedName:
                    objDistinguishedName.Encode(distinguishedName, X500NameFlags.XCN_CERT_NAME_STR_COMMA_FLAG);
                    break;
                case CommonNameTypes.DNSFQDN:
                    objDistinguishedName.Encode($"CN={commonNames[CommonNameTypes.DNSFQDN]}");
                    break;
                case CommonNameTypes.Hostname:
                    objDistinguishedName.Encode($"CN={commonNames[CommonNameTypes.Hostname]}");
                    break;
                case CommonNameTypes.UserPrincipalName:
                    objDistinguishedName.Encode($"CN={commonNames[CommonNameTypes.UserPrincipalName]}");
                    break;
                case CommonNameTypes.SAMAccountName:
                    objDistinguishedName.Encode($"CN={commonNames[CommonNameTypes.SAMAccountName]}");
                    break;
                case CommonNameTypes.UUID:
                    objDistinguishedName.Encode($"CN={commonNames[CommonNameTypes.UUID]}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(designatedSubject), designatedSubject, null);
            }

            return objDistinguishedName;
        }

        private static void AddSpnAlternativeNames(Dictionary<CommonNameTypes, string> commonNames, CAlternativeNames objAlternativeNames)
        {
            // Add AMT SPNs
            if (commonNames.ContainsKey(CommonNameTypes.AMTHTTP))
            {
                // HTTP
                var amtHttpSpn = new CAlternativeName();
                amtHttpSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTHTTP]);
                objAlternativeNames.Add(amtHttpSpn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.AMTHTTPS))
            {
                // HTTPS
                var amtHttpsSpn = new CAlternativeName();
                amtHttpsSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTHTTPS]);
                objAlternativeNames.Add(amtHttpsSpn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.AMTRMCP))
            {
                // AMTRMCP
                var amtRmcpSpn = new CAlternativeName();
                amtRmcpSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTRMCP]);
                objAlternativeNames.Add(amtRmcpSpn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.AMTSecureRMCP))
            {
                // AMTSecureRMCP
                var amtSecureRmcpSpn = new CAlternativeName();
                amtSecureRmcpSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTSecureRMCP]);
                objAlternativeNames.Add(amtSecureRmcpSpn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.AMTTCPRedirect))
            {
                // AMTTCPRedirect
                var amtTcpRedirectSpn = new CAlternativeName();
                amtTcpRedirectSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTTCPRedirect]);
                objAlternativeNames.Add(amtTcpRedirectSpn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.AMTTLSRedirect))
            {
                // AMTTLSRedirect
                var amtTlsRedirectSpn = new CAlternativeName();
                amtTlsRedirectSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTTLSRedirect]);
                objAlternativeNames.Add(amtTlsRedirectSpn);
            }
            if (commonNames.ContainsKey(CommonNameTypes.AMTVNCKVM))
            {
                // AMTVNCKVM
                var amtVncKvmSpn = new CAlternativeName();
                amtVncKvmSpn.InitializeFromString(AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME, commonNames[CommonNameTypes.AMTVNCKVM]);
                objAlternativeNames.Add(amtVncKvmSpn);
            }
        }
    }
}
