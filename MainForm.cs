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

namespace MeshCentralSatellite
{
    public partial class MainForm : Form
    {
        public LoginForm loginForm = null;
        public int currentPanel = 0;
        public DateTime refreshTime = DateTime.Now;
        public MeshCentralSatelliteServer server = null;
        public MeshSatelliteService service = null;
        public MeshCentralServer meshcentral = null;
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
        public string argCA = null;
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
            if (meshcentral == null) return false;
            return meshcentral.RemoteCertificateValidation(certificate, chain, sslPolicyErrors);
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
                        if (key == "ca") { argCA = val; }
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
                if ((arg.Length > 6) && (arg.Substring(0, 6).ToLower() == "-host:")) { argServerName = arg.Substring(6); argflags |= 1; }
                if ((arg.Length > 6) && (arg.Substring(0, 6).ToLower() == "-user:")) { argUserName = arg.Substring(6); argflags |= 2; }
                if ((arg.Length > 6) && (arg.Substring(0, 6).ToLower() == "-pass:")) { argPassword = arg.Substring(6); argflags |= 4; }
                if ((arg.Length > 4) && (arg.Substring(0, 4).ToLower() == "-ca:")) { argCA = arg.Substring(4); }
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
            connectToolStripMenuItem.Enabled = ((!serviceInstalled) && (meshcentral == null));
            disconnectToolStripMenuItem.Enabled = ((!serviceInstalled) && (meshcentral != null));

            // Update service menu
            startToolStripMenuItem.Enabled = ((meshcentral == null) && (serviceInstalled) && (status == ServiceControllerStatus.Stopped));
            stopToolStripMenuItem.Enabled = ((meshcentral == null) && (serviceInstalled) && (status == ServiceControllerStatus.Running));
            installToolStripMenuItem.Enabled = (meshcentral == null) && !serviceInstalled;
            uninstallToolStripMenuItem.Enabled = (meshcentral == null) && serviceInstalled;

            // Update testing menu
            createTestComputerToolStripMenuItem.Enabled = ((meshcentral != null) || ((serviceInstalled) && (status == ServiceControllerStatus.Running)));
            removeTestComputerToolStripMenuItem.Enabled = ((meshcentral != null) || ((serviceInstalled) && (status == ServiceControllerStatus.Running)));
            listCertificateAuthoritiesToolStripMenuItem.Enabled = ((meshcentral != null) || ((serviceInstalled) && (status == ServiceControllerStatus.Running)));

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
            Log("Started up.");
            updateInfo();
            if ((connectToolStripMenuItem.Enabled) && (argServerName != null) && (argUserName != null) && (argPassword != null))
            {
                connectToolStripMenuItem_Click(this, null);
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loginForm != null) return;
            loginForm = new LoginForm(this);
            loginForm.Show(this);
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
            meshcentral = null;
            eventsListView.Items.Clear();
            this.Text = title;
            Log("Disconnected by user");
            updateInfo();
        }

        private void Server_onMessage(string msg)
        {
            Log(msg);
        }

        public void ConnectedToServer()
        {
            server = new MeshCentralSatelliteServer(argServerName, argUserName, argPassword, null, meshcentral);
            server.onStateChanged += Server_onStateChanged;
            server.onMessage += Server_onMessage;
            server.onEvent += Server_onEvent;
            server.SetCertificateAuthority(argCA);
            server.Start();
        }

        private void Server_onEvent(MeshCentralSatelliteServer.ServerEvent e)
        {
            AddEvent(e.icon, e.time, e.msg);
        }

        private void Server_onStateChanged(int state)
        {
            if (meshcentral == null) return;
            if (this.InvokeRequired) { this.Invoke(new MeshCentralServer.onStateChangedHandler(Server_onStateChanged), state); return; }

            if (state == 0)
            {
                this.Text = title;
                if (server == null) return;
                server.onMessage -= Server_onMessage;
                server.Stop();
                server = null;
                meshcentral = null;
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
                        if (key == "ca") { f.ca = val; }
                        if ((key == "log") && ((val == "1") || (val.ToLower() == "true"))) { f.log = true; }
                        if ((key == "debug") && ((val == "1") || (val.ToLower() == "true"))) { f.debug = true; }
                        if ((key == "ignorecert") && ((val == "1") || (val.ToLower() == "true"))) { f.ignoreCert = true; }
                    }
                }
            }
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                config = "";
                config += "host=" + f.host + "\r\n";
                config += "user=" + f.user + "\r\n";
                config += "pass=" + f.pass + "\r\n";
                if (f.ca != "") { config += "ca=" + f.ca + "\r\n"; }
                if (f.log) { config += "log=1\r\n"; }
                if (f.debug) { config += "debug=1\r\n"; }
                if (f.ignoreCert) { config += "ignoreCert=1\r\n"; }
                try { File.WriteAllText(configFile, config); } catch (Exception) {
                    MessageBox.Show(this, "Unable to write config.txt file.", "Settings Error");
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

        private void listCertificateAuthoritiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.EventCertificateAuthorities();
            }
            else
            {
                localPipeClient.Send("ListCertificateAuthorities");
            }
        }

        private void clearConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainTextBox.Text = "";
        }

    }
}
