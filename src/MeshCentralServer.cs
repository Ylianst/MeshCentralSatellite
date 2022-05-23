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
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Deployment.Application;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;
using System.Reflection;

namespace MeshCentralSatellite
{

    public class MeshCentralServer
    {
        public Uri wsurl = null;
        private string user = null;
        private string pass = null;
        private string token = null;
        private webSocketClient wc = null;
        //private System.Timers.Timer procTimer = new System.Timers.Timer(5000);
        private int constate = 0;
        public string disconnectCause = null;
        public string disconnectMsg = null;
        public bool disconnectEmail2FA = false;
        public bool disconnectEmail2FASent = false;
        public bool disconnectSms2FA = false;
        public bool disconnectSms2FASent = false;
        public X509Certificate2 disconnectCert;
        public string authCookie = null;
        public string rauthCookie = null;
        public string loginCookie = null;
        public string wshash = null;
        public string certHash = null;
        public string okCertHash = null;
        public string okCertHash2 = null;
        public bool debug = false;
        public bool tlsdump = false;
        public bool ignoreCert = false;
        public string userid = null;
        public string username = null;
        public int twoFactorCookieDays = 0;
        public Dictionary<string, ulong> userRights = null;
        public Dictionary<string, string> userGroups = null;
        private JavaScriptSerializer JSON = new JavaScriptSerializer();
        public int features = 0; // Bit flags of server features
        public int features2 = 0; // Bit flags of server features
        string debugFilePath = null;

        public int connectionState { get { return constate; } }

        // Mesh Rights
        /*
        const MESHRIGHT_EDITMESH = 1;
        const MESHRIGHT_MANAGEUSERS = 2;
        const MESHRIGHT_MANAGECOMPUTERS = 4;
        const MESHRIGHT_REMOTECONTROL = 8;e
        const MESHRIGHT_AGENTCONSOLE = 16;
        const MESHRIGHT_SERVERFILES = 32;
        const MESHRIGHT_WAKEDEVICE = 64;
        const MESHRIGHT_SETNOTES = 128;
        const MESHRIGHT_REMOTEVIEWONLY = 256;
        const MESHRIGHT_NOTERMINAL = 512;
        const MESHRIGHT_NOFILES = 1024;
        const MESHRIGHT_NOAMT = 2048;
        const MESHRIGHT_DESKLIMITEDINPUT = 4096;
        const MESHRIGHT_LIMITEVENTS = 8192;
        const MESHRIGHT_CHATNOTIFY = 16384;
        const MESHRIGHT_UNINSTALL = 32768;
        */

        public static void saveToRegistry(string name, string value)
        {
            try { Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\OpenSource\MeshRouter", name, value); } catch (Exception) { }
        }
        public static string loadFromRegistry(string name)
        {
            try { return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\OpenSource\MeshRouter", name, "").ToString(); } catch (Exception) { return ""; }
        }

        public static string GetProxyForUrlUsingPac(string DestinationUrl, string PacUri)
        {
            IntPtr WinHttpSession = Win32Api.WinHttpOpen("User", Win32Api.WINHTTP_ACCESS_TYPE_DEFAULT_PROXY, IntPtr.Zero, IntPtr.Zero, 0);

            Win32Api.WINHTTP_AUTOPROXY_OPTIONS ProxyOptions = new Win32Api.WINHTTP_AUTOPROXY_OPTIONS();
            Win32Api.WINHTTP_PROXY_INFO ProxyInfo = new Win32Api.WINHTTP_PROXY_INFO();

            ProxyOptions.dwFlags = Win32Api.WINHTTP_AUTOPROXY_CONFIG_URL;
            ProxyOptions.dwAutoDetectFlags = (Win32Api.WINHTTP_AUTO_DETECT_TYPE_DHCP | Win32Api.WINHTTP_AUTO_DETECT_TYPE_DNS_A);
            ProxyOptions.lpszAutoConfigUrl = PacUri;

            // Get Proxy 
            bool IsSuccess = Win32Api.WinHttpGetProxyForUrl(WinHttpSession, DestinationUrl, ref ProxyOptions, ref ProxyInfo);
            Win32Api.WinHttpCloseHandle(WinHttpSession);

            if (IsSuccess)
            {
                return ProxyInfo.lpszProxy;
            }
            else
            {
                Console.WriteLine("Error: {0}", Win32Api.GetLastError());
                return null;
            }
        }

        // Parse the URL query parameters and returns a collection
        public static NameValueCollection GetQueryStringParameters()
        {
            NameValueCollection nameValueTable = new NameValueCollection();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string queryString = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
                nameValueTable = HttpUtility.ParseQueryString(queryString);
            }
            return (nameValueTable);
        }

        // Starts the routing server, called when the start button is pressed
        public void connect(Uri wsurl, string user, string pass, string token)
        {
            FileInfo fi = new FileInfo(Path.Combine(Assembly.GetExecutingAssembly().Location));
            String executablePath = fi.Directory.FullName;
            debugFilePath = Path.Combine(executablePath, "debug.log");

            JSON.MaxJsonLength = 217483647;
            this.user = user;
            this.pass = pass;
            this.token = token;
            this.wsurl = wsurl;

            // Setup extra headers if needed
            Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
            if (user != null && pass != null && token != null) {
                extraHeaders.Add("x-meshauth", Base64Encode(user) + "," + Base64Encode(pass) + "," + Base64Encode(token));
            } else if (user != null && pass != null) {
                extraHeaders.Add("x-meshauth", Base64Encode(user) + "," + Base64Encode(pass));
            }

            wc = new webSocketClient();
            wc.extraHeaders = extraHeaders;
            wc.onStateChanged += new webSocketClient.onStateChangedHandler(changeStateEx);
            wc.onStringData += new webSocketClient.onStringDataHandler(processServerData);
            wc.onDebugMessage += Wc_onDebugMessage;
            //Debug("#" + counter + ": Connecting web socket to: " + wsurl.ToString());
            wc.TLSCertCheck = webSocketClient.TLSCertificateCheck.Verify;
            wc.Start(wsurl, okCertHash, okCertHash2);
            logdebug("Connect-" + wsurl);
            wc.debug = debug;
            wc.tlsdump = tlsdump;
            wc.TLSCertCheck = (ignoreCert) ? webSocketClient.TLSCertificateCheck.Ignore : webSocketClient.TLSCertificateCheck.Verify;
        }

        private void Wc_onDebugMessage(webSocketClient sender, string msg)
        {
            logdebug(msg);
        }

        private void logdebug(string msg) { if (debug || tlsdump) { try { File.AppendAllText(debugFilePath, msg + "\r\n"); } catch (Exception) { } } }
        private void tcpdebug(string msg) { if (tlsdump) { try { File.AppendAllText(debugFilePath, msg + "\r\n"); } catch (Exception) { } } }

        public void disconnect()
        {
            if (wc != null)
            {
                wc.Dispose();
                wc = null;
                tcpdebug("Disconnect");
            }
        }

        public void sendCommand(string cmd)
        {
            if (wc != null)
            {
                logdebug("sendCommand: " + cmd);
                wc.SendString(cmd);
            }
        }

        public void sendCommand(Dictionary<string, object> cmd)
        {
            if (wc != null)
            {
                string c = JSON.Serialize(cmd);
                logdebug("sendCommand: " + c);
                wc.SendString(c);
            }
        }

        public void refreshCookies()
        {
            if (wc != null) {
                logdebug("RefreshCookies");
                wc.SendString("{\"action\":\"authcookie\"}");
                wc.SendString("{\"action\":\"logincookie\"}");
            }
        }

        public void sendSatelliteSetFlags(int flags)
        {
            // 1 = This is a satellite session
            // 2 = Has 802.1x support
            logdebug("Set satellite flags: " + flags);
            wc.SendString("{\"action\":\"satellite\",\"setFlags\":" + flags + "}");
        }

        public void processServerData(webSocketClient sender, string data, int orglen)
        {
            logdebug("ServerData -" + data);

            // Parse the received JSON
            Dictionary<string, object> jsonAction = new Dictionary<string, object>();
            try
            {
                jsonAction = JSON.Deserialize<Dictionary<string, object>>(data);
            } catch (Exception ex) {
                logdebug("processServerData JSON Deserialize error: \r\n" + ex.ToString());
                logdebug("Invalid data (" + data.Length + "): \r\n" + data);
                return;
            }
            if (jsonAction == null || jsonAction["action"].GetType() != typeof(string)) return;

            try
            {
                string action = jsonAction["action"].ToString();
                switch (action)
                {
                    case "pong":
                        {
                            // NOP
                            break;
                        }
                    case "ping":
                        {
                            // Send pong back
                            if (wc != null) { wc.SendString("{\"action\":\"pong\"}"); }
                            break;
                        }
                    case "close":
                        {
                            disconnectCause = jsonAction["cause"].ToString();
                            disconnectMsg = jsonAction["msg"].ToString();
                            if (jsonAction.ContainsKey("email2fa")) { disconnectEmail2FA = (bool)jsonAction["email2fa"]; } else { disconnectEmail2FA = false; }
                            if (jsonAction.ContainsKey("email2fasent")) { disconnectEmail2FASent = (bool)jsonAction["email2fasent"]; } else { disconnectEmail2FASent = false; }
                            if (jsonAction.ContainsKey("sms2fa")) { disconnectSms2FA = (bool)jsonAction["sms2fa"]; } else { disconnectSms2FA = false; }
                            if (jsonAction.ContainsKey("sms2fasent")) { disconnectSms2FASent = (bool)jsonAction["sms2fasent"]; } else { disconnectSms2FASent = false; }
                            if (jsonAction.ContainsKey("twoFactorCookieDays") && (jsonAction["twoFactorCookieDays"].GetType() == typeof(int))) { twoFactorCookieDays = (int)jsonAction["twoFactorCookieDays"]; }
                            break;
                        }
                    case "serverinfo":
                        {
                            // Get the bit flags of server features
                            Dictionary<string, object> serverinfo = (Dictionary<string, object>)jsonAction["serverinfo"];
                            if (serverinfo.ContainsKey("features") && (serverinfo["features"].GetType() == typeof(int))) { features = (int)serverinfo["features"]; }
                            if (serverinfo.ContainsKey("features2") && (serverinfo["features2"].GetType() == typeof(int))) { features2 = (int)serverinfo["features2"]; }
                            wc.SendString("{\"action\":\"authcookie\"}");
                            break;
                        }
                    case "authcookie":
                        {
                            authCookie = jsonAction["cookie"].ToString();
                            rauthCookie = jsonAction["rcookie"].ToString();
                            changeState(2);

                            if (sender.RemoteCertificate != null)
                            {
                                certHash = webSocketClient.GetMeshCertHash(new X509Certificate2(sender.RemoteCertificate));
                            }

                            break;
                        }
                    case "logincookie":
                        {
                            loginCookie = jsonAction["cookie"].ToString();
                            if (onLoginTokenChanged != null) { onLoginTokenChanged(); }
                            break;
                        }
                    case "userinfo":
                        {
                            Dictionary<string, object> userinfo = (Dictionary<string, object>)jsonAction["userinfo"];
                            userid = (string)userinfo["_id"];
                            if (userinfo.ContainsKey("name")) { username = (string)userinfo["name"]; }
                            userRights = new Dictionary<string, ulong>();
                            if (userinfo.ContainsKey("links"))
                            {
                                Dictionary<string, object> userLinks = (Dictionary<string, object>)userinfo["links"];
                                foreach (string i in userLinks.Keys)
                                {
                                    Dictionary<string, object> userLinksEx = (Dictionary<string, object>)userLinks[i];
                                    if (userLinksEx.ContainsKey("rights"))
                                    {
                                        userRights[i] = ulong.Parse(userLinksEx["rights"].ToString());
                                    }
                                }
                            }
                            break;
                        }
                    case "event":
                        {
                            Dictionary<string, object> ev = (Dictionary<string, object>)jsonAction["event"];
                            string action2 = ev["action"].ToString();
                            /*
                            switch (action2)
                            {

                            }
                            */
                            break;
                        }
                    case "msg":
                        {
                            if (jsonAction.ContainsKey("type"))
                            {
                                string type = (string)jsonAction["type"];
                            }
                            break;
                        }
                    case "twoFactorCookie":
                        {
                            if (jsonAction.ContainsKey("cookie"))
                            {
                                string cookie = (string)jsonAction["cookie"];
                                if (onTwoFactorCookie != null) { onTwoFactorCookie(cookie); }
                            }
                            break;
                        }
                    case "meshToolInfo":
                        {
                            if (onToolUpdate == null) return;
                            if (jsonAction.ContainsKey("hash") && jsonAction.ContainsKey("url"))
                            {
                                // MeshCentral Router hash on the server
                                string hash = (string)jsonAction["hash"];

                                // Hash our own executable
                                byte[] selfHash;
                                using (var sha384 = SHA384Managed.Create()) { using (var stream = File.OpenRead(System.Reflection.Assembly.GetEntryAssembly().Location)) { selfHash = sha384.ComputeHash(stream); } }
                                string selfExecutableHashHex = BitConverter.ToString(selfHash).Replace("-", string.Empty).ToLower();

                                // Get login key
                                string url = jsonAction["url"] + "&auth=" + authCookie;
                                if (url.StartsWith("*/")) { url = "https://" + wsurl.Authority + url.Substring(1); }
                                string loginkey = getValueFromQueryString(wsurl.Query, "key");
                                if (loginkey != null) { url += ("&key=" + loginkey); }

                                // Server TLS certificate hash
                                string serverhash = null;
                                if (jsonAction.ContainsKey("serverhash")) { serverhash = jsonAction["serverhash"].ToString(); }

                                // If the hashes don't match, event the tool update with URL
                                if (selfExecutableHashHex != hash) { onToolUpdate((string)url, (string)jsonAction["hash"], (int)jsonAction["size"], serverhash); }
                            }
                            break;
                        }
                        case "satellite":
                        {
                            if (onSatelliteMessage != null) { onSatelliteMessage(jsonAction); }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                logdebug(ex.ToString());
            }
        }

        private static string getValueFromQueryString(string query, string name)
        {
            if ((query == null) || (name == null)) return null;
            int i = query.IndexOf("?" + name + "=");
            if (i == -1) { i = query.IndexOf("&" + name + "="); }
            if (i == -1) return null;
            string r = query.Substring(i + name.Length + 2);
            i = r.IndexOf("&");
            if (i >= 0) { r = r.Substring(0, i); }
            return r;
        }

        public delegate void onStateChangedHandler(int state);
        public event onStateChangedHandler onStateChanged;
        public void changeState(int newState) { if (constate != newState) { constate = newState; if (onStateChanged != null) { onStateChanged(constate); } } }

        public delegate void onSatelliteMessageHandler(Dictionary<string, object> message);
        public event onSatelliteMessageHandler onSatelliteMessage;

        private void changeStateEx(webSocketClient sender, webSocketClient.ConnectionStates newState)
        {
            if (newState == webSocketClient.ConnectionStates.Disconnected) {
                if (sender.failedTlsCert != null) { certHash = null; disconnectMsg = "cert"; disconnectCert = sender.failedTlsCert; }
                changeState(0);
            }
            if (newState == webSocketClient.ConnectionStates.Connecting) { changeState(1); }
            if (newState == webSocketClient.ConnectionStates.Connected) { }
        }

        public delegate void onLoginTokenChangedHandler();
        public event onLoginTokenChangedHandler onLoginTokenChanged;
        public delegate void twoFactorCookieHandler(string cookie);
        public event twoFactorCookieHandler onTwoFactorCookie;
        public delegate void toolUpdateHandler(string url, string hash, int size, string serverhash);
        public event toolUpdateHandler onToolUpdate;

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public bool RemoteCertificateValidation(X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (ignoreCert) return true;
            if (connectionState < 2)
            {
                // Normal certificate check
                if (chain.Build(new X509Certificate2(certificate)) == true) { certHash = webSocketClient.GetMeshKeyHash(certificate); return true; }
                if ((okCertHash != null) && ((okCertHash == certificate.GetCertHashString()) || (okCertHash == webSocketClient.GetMeshKeyHash(certificate)) || (okCertHash == webSocketClient.GetMeshCertHash(certificate)))) { certHash = webSocketClient.GetMeshKeyHash(certificate); return true; }
                if ((okCertHash2 != null) && ((okCertHash2 == certificate.GetCertHashString()) || (okCertHash2 == webSocketClient.GetMeshKeyHash(certificate)) || (okCertHash2 == webSocketClient.GetMeshCertHash(certificate)))) { certHash = webSocketClient.GetMeshKeyHash(certificate); return true; }
                certHash = null;
                disconnectMsg = "cert";
                disconnectCert = new X509Certificate2(certificate);
            }
            else
            {
                if ((certHash != null) && ((certHash == certificate.GetCertHashString()) || (certHash == webSocketClient.GetMeshKeyHash(certificate)) || (certHash == webSocketClient.GetMeshCertHash(certificate)))) { return true; }
            }
            return false;
        }

    }
}