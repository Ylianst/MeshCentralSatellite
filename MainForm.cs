/*
Copyright 2009-2022 Intel Corporation

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
using System.Net;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MeshCentralSatellite
{
    public partial class MainForm : Form
    {
        public int currentPanel = 0;
        public DateTime refreshTime = DateTime.Now;
        public MeshCentralSatelliteServer server = null;
        public MeshSatelliteService service = null;
        public X509Certificate2 lastBadConnectCert = null;
        public LocalPipeClient localPipeClient = null;
        public string title;
        public string[] args;
        public bool log = false;
        public bool debug = false;
        public bool tlsdump = false;
        public bool autoLogin = false;
        public bool ignoreCert = false;
        public bool forceExit = false;
        public bool sendEmailToken = false;
        public bool sendSMSToken = false;
        public Uri authLoginUrl = null;
        public string acceptableCertHash = null;
        public string argServerName = null;
        public string argUserName = null;
        public string argPassword = null;
        public string argDevNameType = null;
        public string argCAName = null;
        public string argCATemplate = null;
        public string argCertCommonName = null;
        public string argCertAltNames = null;
        public List<string> argDevSecurityGroups = new List<string>();
        public String executablePath = null;

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void DeleteSubKeyTree(RegistryKey key, string subkey) { if (key.OpenSubKey(subkey) == null) { return; } DeleteSubKeyTree(key, subkey); }

        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if ((server == null) || (server.meshcentral == null)) return false;
            return server.meshcentral.RemoteCertificateValidation(certificate, chain, sslPolicyErrors);
        }

        public MainForm(string[] args)
        {
            // Set TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

            this.args = args;
            InitializeComponent();
            Translate.TranslateControl(this);
            pictureBox1.SendToBack();
            Version version = Assembly.GetEntryAssembly().GetName().Version;

            // Get our assembly path
            FileInfo fi = new FileInfo(Path.Combine(Assembly.GetExecutingAssembly().Location));
            executablePath = fi.Directory.FullName;

            // Parse the config.txt file
            int argflags = 0;
            string config = null;
            try { config = File.ReadAllText("config.txt"); } catch (Exception) { }
            if (config == null) { try { config = File.ReadAllText(Path.Combine(executablePath, "config.txt")); } catch (Exception) { } }
            if (config != null)
            {
                string[] configLines = config.Replace("\r\n", "\n").Split('\n');
                foreach (string configLine in configLines)
                {
                    int i = configLine.IndexOf('=');
                    if (i > 0)
                    {
                        string key = configLine.Substring(0, i).ToLower();
                        string val = configLine.Substring(i + 1);
                        if (key == "host") { argServerName = val; argflags |= 1; }
                        if (key == "user") { argUserName = val; argflags |= 2; }
                        if (key == "pass") { argPassword = val; argflags |= 4; }
                        if (key == "devnametype") { argDevNameType = val; }
                        if (key == "caname") { argCAName = val; }
                        if (key == "catemplate") { argCATemplate = val; }
                        if (key == "certcommonname") { argCertCommonName = val; }
                        if (key == "certaltnames") { argCertAltNames = val; }
                        if (key == "devsecuritygroup") { argDevSecurityGroups.Add(val); }
                        if ((key == "log") && ((val == "1") || (val.ToLower() == "true"))) { log = true; }
                        if ((key == "debug") && ((val == "1") || (val.ToLower() == "true"))) { debug = true; }
                        if ((key == "ignorecert") && ((val == "1") || (val.ToLower() == "true"))) { ignoreCert = true; }
                    }
                }
            }

            string update = null;
            string delete = null;
            bool runAsService = false;
            foreach (string arg in this.args) {
                if (arg.ToLower() == "-log") { log = true; }
                if (arg.ToLower() == "-debug") { debug = true; }
                if (arg.ToLower() == "-tlsdump") { tlsdump = true; }
                if (arg.ToLower() == "-ignorecert") { ignoreCert = true; }
                if (arg.ToLower() == "-service") { runAsService = true; }
                if (arg.ToLower() == "-native") { webSocketClient.nativeWebSocketFirst = true; }
                if ((arg.Length > 8) && (arg.Substring(0, 8).ToLower() == "-update:")) { update = arg.Substring(8); }
                if ((arg.Length > 8) && (arg.Substring(0, 8).ToLower() == "-delete:")) { delete = arg.Substring(8); }
            }
            autoLogin = (argflags == 7);

            // To make it easy to debug when running as a service, we emulate running as a service with the -service switch
            if (runAsService)
            {
                service = new MeshSatelliteService();
                service.StartServer();
                return;
            }

            if (update != null) {
                // New args
                ArrayList args2 = new ArrayList();
                foreach (string a in args) { if (a.StartsWith("-update:") == false) { args2.Add(a); } }

                // Remove ".update.exe" and copy
                System.Threading.Thread.Sleep(1000);
                File.Copy(Assembly.GetEntryAssembly().Location, update, true);
                System.Threading.Thread.Sleep(1000);
                Process.Start(update, string.Join(" ", (string[])args2.ToArray(typeof(string))) + " -delete:" + Assembly.GetEntryAssembly().Location);
                this.forceExit = true;
                Application.Exit();
                return;
            }

            if (delete != null) { try { System.Threading.Thread.Sleep(1000); File.Delete(delete); } catch (Exception) { } }

            title = this.Text;

            // Start the local pipe client, used to receive debug messages from the server.
            localPipeClient = new LocalPipeClient("MeshCentralSatelliteDebugPipe");
            localPipeClient.onMessage += LocalPipeClient_onMessage;
            localPipeClient.Start();

            // Show version
            versionToolStripStatusLabel.Text = "v" + Application.ProductVersion;
        }

        private void LocalPipeClient_onMessage(string msg)
        {
            if (msg.StartsWith("log|")) { Log("Service: " + msg.Substring(4)); }
            if (msg.StartsWith("event|")) {
                string[] strSplit = msg.Split('|');
                if (strSplit.Length == 4)
                {
                    int icon = int.Parse(strSplit[1]);
                    DateTime time = new DateTime(long.Parse(strSplit[2]));
                    string emsg = strSplit[3];
                    AddEvent(icon, time, "Service: " + emsg);
                }
            }
        }

        public void updateInfo()
        {
            // See if the service is installed
            bool serviceInstalled = false;
            foreach (ServiceController sc in ServiceController.GetServices())
            {
                if ((sc.ServiceName == "MeshCentral Satellite Server") && (sc.StartType != ServiceStartMode.Disabled)) { serviceInstalled = true; break; }
            }

            ServiceControllerStatus status = ServiceControllerStatus.Stopped;
            if (serviceInstalled == true)
            {
                ServiceController controller = new ServiceController("MeshCentral Satellite Server");
                try { status = controller.Status; } catch (InvalidOperationException) { serviceInstalled = false; }
            }

            // Update the file menu
            connectToolStripMenuItem.Enabled = ((!serviceInstalled) && (server == null));
            disconnectToolStripMenuItem.Enabled = ((!serviceInstalled) && (server != null));

            // Update service menu
            startToolStripMenuItem.Enabled = ((server == null) && (serviceInstalled) && (status == ServiceControllerStatus.Stopped));
            stopToolStripMenuItem.Enabled = ((server == null) && (serviceInstalled) && (status == ServiceControllerStatus.Running));
            installToolStripMenuItem.Enabled = (server == null) && !serviceInstalled;
            uninstallToolStripMenuItem.Enabled = (server == null) && serviceInstalled;

            // Update testing menu
            createTestComputerToolStripMenuItem.Enabled = ((server != null) || ((serviceInstalled) && (status == ServiceControllerStatus.Running)));
            removeTestComputerToolStripMenuItem.Enabled = ((server != null) || ((serviceInstalled) && (status == ServiceControllerStatus.Running)));
            testCertificateAuthorityToolStripMenuItem.Enabled = ((server != null) || ((serviceInstalled) && (status == ServiceControllerStatus.Running)));

            if (serviceInstalled)
            {
                mainToolStripStatusLabel.Text = "Service Installed: " + status;
            }
            else
            {
                // Update the status bar
                if (server == null)
                {
                    mainToolStripStatusLabel.Text = "Disconnected";
                }
                else
                {
                    if (server.connectionState == 0) { mainToolStripStatusLabel.Text = "Disconnected"; }
                    if (server.connectionState == 1) { mainToolStripStatusLabel.Text = "Connecting..."; }
                    if (server.connectionState == 2) { mainToolStripStatusLabel.Text = "Connected"; }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (service != null) return;
            Log("Started up using " + RuntimeInformation.FrameworkDescription);
            if (GetDotNetVersion() < 528040) {
                MessageBox.Show(this, "This application requires .NET framework 4.8 or higher", this.title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log("WARNING: This application requires .NET framework 4.8 or higher");
                return;
            }

            updateInfo();
            if ((connectToolStripMenuItem.Enabled) && (argServerName != null) && (argUserName != null) && (argPassword != null))
            {
                connectToolStripMenuItem_Click(this, null);
            }
        }

        private static int GetDotNetVersion()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
            {
                var value = key?.GetValue("Release");
                if (value == null) return 0;
                return Convert.ToInt32(value); // 528040 is .NET 4.8
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server != null) return;
            try
            {
                // Create & start server
                server = new MeshCentralSatelliteServer(argServerName, argUserName, argPassword, null);
                server.devNameType = argDevNameType;
                server.devSecurityGroups = argDevSecurityGroups;
                server.debug = debug;
                server.onStateChanged += Server_onStateChanged;
                server.onMessage += Server_onMessage;
                server.onEvent += Server_onEvent;
                server.ignoreCert = ignoreCert;
                server.SetCertificateAuthority(argCAName, argCATemplate, argCertCommonName, argCertAltNames);
                server.Start();
            }
            catch (Exception ex)
            {
                Log("Exception" + ex.ToString());
                throw ex;
            }
        }

        private delegate void LogHandler(string msg);
        public void Log(string msg)
        {
            if (this.InvokeRequired) { this.Invoke(new LogHandler(Log), msg); return; }
            if (log) { try { File.AppendAllText("log.txt", DateTime.Now.ToString("HH:mm:tt.ffff") + ": " + msg + "\r\n"); } catch (Exception) { } }
            if (debug) { try { File.AppendAllText("debug.log", DateTime.Now.ToString("HH:mm:tt.ffff") + ": " + msg + "\r\n"); } catch (Exception) { } }
            mainTextBox.AppendText(DateTime.Now.ToLongTimeString() + " - " + msg + "\r\n");
        }

        private delegate void AddEventHandler(int icon, DateTime time, string msg);
        public void AddEvent(int icon, DateTime time, string msg)
        {
            if (InvokeRequired) { Invoke(new AddEventHandler(AddEvent), icon, time, msg); return; }
            Log(msg);
            if (msg.StartsWith("Service: ")) { msg = msg.Substring(9); }
            eventsListView.Items.Insert(0, new ListViewItem(new string[2] { time.ToString(), msg }, icon));
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server == null) return;
            server.onStateChanged -= Server_onStateChanged;
            server.onMessage -= Server_onMessage;
            server.Stop();
            server = null;
            eventsListView.Items.Clear();
            this.Text = title;
            Log("Disconnected by user");
            updateInfo();
        }

        private void Server_onMessage(string msg)
        {
            Log(msg);
        }

        private void Server_onEvent(MeshCentralSatelliteServer.ServerEvent e)
        {
            AddEvent(e.icon, e.time, e.msg);
        }

        private void Server_onStateChanged(int state)
        {
            if (server == null) return;
            if (this.InvokeRequired) { this.Invoke(new MeshCentralServer.onStateChangedHandler(Server_onStateChanged), state); return; }

            if (state == 0)
            {
                this.Text = title;
                if (server == null) return;
                server.onMessage -= Server_onMessage;
                server.Stop();
                server = null;
                eventsListView.Items.Clear();
                Log("Disconnected by server");
            }
            updateInfo();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Starting service.");
            ServiceController controller = new ServiceController("MeshCentral Satellite Server");
            try { controller.Start(); } catch (Exception) {
                MessageBox.Show(this, "Unable to start service.", "Service");
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Stopping service.");
            ServiceController controller = new ServiceController("MeshCentral Satellite Server");
            try { controller.Stop(); } catch (Exception) {
                MessageBox.Show(this, "Unable to stop service.", "Service");
            }
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Installing service.");
            Process.Start(Assembly.GetExecutingAssembly().Location, "-install");
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Uninstalling service.");
            Process.Start(Assembly.GetExecutingAssembly().Location, "-uninstall");
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateInfo();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm();

            // Parse the config.txt file
            string config = null;
            string configFile = "config.txt";
            try { config = File.ReadAllText(configFile); } catch (Exception) { }
            if (config == null) { try { configFile = Path.Combine(executablePath, "config.txt"); config = File.ReadAllText(configFile); } catch (Exception) { } }
            if (config != null)
            {
                string[] configLines = config.Replace("\r\n", "\n").Split('\n');
                string caname = null;
                string catemplate = null;
                List<string> xdevSecurityGroups = new List<string>();
                foreach (string configLine in configLines)
                {
                    int i = configLine.IndexOf('=');
                    if (i > 0)
                    {
                        string key = configLine.Substring(0, i).ToLower();
                        string val = configLine.Substring(i + 1);
                        if (key == "host") { f.host = val; }
                        if (key == "user") { f.user = val; }
                        if (key == "pass") { f.pass = val; }
                        if (key == "devnametype") { f.devNameType = val; }
                        if (key == "caname") { caname = val; }
                        if (key == "catemplate") { catemplate = val; }
                        if (key == "certcommonname") { f.certCommonName = val; }
                        if (key == "certaltnames") { f.certAltNames = val; }
                        if (key == "devsecuritygroup") { xdevSecurityGroups.Add(val); }
                        if ((key == "log") && ((val == "1") || (val.ToLower() == "true"))) { f.log = true; }
                        if ((key == "debug") && ((val == "1") || (val.ToLower() == "true"))) { f.debug = true; }
                        if ((key == "ignorecert") && ((val == "1") || (val.ToLower() == "true"))) { f.ignoreCert = true; }
                    }
                }
                if (caname != null) { f.setCertificateAuthority(caname, catemplate); }
                f.securityGroups = xdevSecurityGroups;
            }
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                bool restartServer = false;
                if (server != null)
                {
                    // If connected locally, disconnect now
                    restartServer = true;
                    disconnectToolStripMenuItem_Click(this, null);
                }

                // Update our own state
                argServerName = f.host;
                argUserName = f.user;
                argPassword = f.pass;
                argDevNameType = f.devNameType;
                if (f.caname != "") { argCAName = f.caname; } else { argCAName = null; }
                argCATemplate = f.catemplate;
                argCertCommonName = f.certCommonName;
                argCertAltNames = f.certAltNames;
                log = f.log;
                debug = f.debug;

                // Update the config.txt file
                config = "";
                config += "host=" + f.host + "\r\n";
                config += "user=" + f.user + "\r\n";
                config += "pass=" + f.pass + "\r\n";
                config += "devNameType=" + f.devNameType + "\r\n";
                if (f.caname != "") {
                    config += "caname=" + f.caname + "\r\n";
                    if (f.catemplate != "") { config += "catemplate=" + f.catemplate + "\r\n"; }
                    config += "certCommonName=" + f.certCommonName + "\r\n";
                    config += "certAltNames=" + f.certAltNames + "\r\n";
                }
                argDevSecurityGroups = f.securityGroups;
                foreach (string securityGroup in argDevSecurityGroups)
                {
                    config += "devSecurityGroup=" + securityGroup + "\r\n";
                }
                if (f.log) { config += "log=1\r\n"; }
                if (f.debug) { config += "debug=1\r\n"; }
                if (f.ignoreCert) { config += "ignoreCert=1\r\n"; }
                try { File.WriteAllText(configFile, config); } catch (Exception) {
                    MessageBox.Show(this, "Unable to write config.txt file.", "Settings Error");
                }

                if (restartServer)
                {
                    connectToolStripMenuItem_Click(this, null);
                }
            }
        }

        private void createTestComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.AddTestComputer();
            }
            else
            {
                localPipeClient.Send("AddTestComputer");
            }
        }

        private void removeTestComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.RemoveTestComputer();
            }
            else
            {
                localPipeClient.Send("RemoveTestComputer");
            }
        }

        private void clearConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainTextBox.Text = "";
        }

        private void testCertificateAuthorityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.TestCertificateAuthority();
            }
            else
            {
                localPipeClient.Send("TestCertificateAuthority");
            }
        }
    }
}
