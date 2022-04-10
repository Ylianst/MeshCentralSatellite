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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading;

namespace MeshCentralSatellite
{
    public partial class MeshSatelliteService : ServiceBase
    {
        private static bool log = false;
        private static bool debug = false;
        private bool ignoreCert = false;
        private MeshCentralSatelliteServer centralServer = null;
        private LocalPipeServer localPipeServer = null;
        private Thread ServerLaunchThread = null;
        private static String logFilePath = null;
        private static String debugFilePath = null;
        private List<MeshCentralSatelliteServer.ServerEvent> eventsList = new List<MeshCentralSatelliteServer.ServerEvent>();

        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            Log("RemoteCertificateValidationCallback");
            if ((centralServer == null) || (centralServer.meshcentral == null)) return false;
            return centralServer.meshcentral.RemoteCertificateValidation(certificate, chain, sslPolicyErrors);
        }

        public MeshSatelliteService()
        {
            InitializeComponent();

            // Setup our event logging system and database connection.
            //MeshLogger.InstallLog("Manageability Server");
            //MeshSettings.Setup("ManageabilityServer");

            // Set TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

            // Exception handling
            System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ExceptionSink);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionEventSink);
            //System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException, true);
        }

        protected override void OnStart(string[] args)
        {
            ServerLaunchThread = new Thread(new ThreadStart(AttemptServerLaunch));
            ServerLaunchThread.IsBackground = true;
            ServerLaunchThread.Start();
            base.OnStart(args);
        }

        // Test the database connection, if it works, start the server
        public void AttemptServerLaunch()
        {
            Program.ServiceLaunchAttempt = true;
            StartServer();
        }

        // Start the actual manageability server
        public void StartServer()
        {
            string argServerName = null;
            string argUserName = null;
            string argPassword = null;
            string argDevNameType = null;
            string argCAName = null;
            string argCATemplate = null;
            string argCertCommonName = null;
            string argCertAltNames = null;

            // Set TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

            // Get our assembly path
            FileInfo fi = new FileInfo(Path.Combine(Assembly.GetExecutingAssembly().Location));
            String executablePath = fi.Directory.FullName;
            logFilePath = Path.Combine(executablePath, "log.txt");
            debugFilePath = Path.Combine(executablePath, "debug.log");

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
                        if ((key == "log") && ((val == "1") || (val.ToLower() == "true"))) { log = true; }
                        if ((key == "debug") && ((val == "1") || (val.ToLower() == "true"))) { debug = true; }
                        if ((key == "ignorecert") && ((val == "1") || (val.ToLower() == "true"))) { ignoreCert = true; }
                    }
                }
            }

            // We can only start logging after log is set to true
            Log("Service started");

            // Start the local pipe server, used to send debug messages to the console.
            localPipeServer = new LocalPipeServer("MeshCentralSatelliteDebugPipe");
            localPipeServer.onMessage += LocalPipeServer_onMessage;
            localPipeServer.onStateChange += LocalPipeServer_onStateChange;

            try
            {
                // Create & start server
                centralServer = new MeshCentralSatelliteServer(argServerName, argUserName, argPassword, null);
                centralServer.devNameType = argDevNameType;
                centralServer.debug = debug;
                centralServer.onStateChanged += CentralServer_onStateChanged;
                centralServer.onMessage += CentralServer_onMessage;
                centralServer.onEvent += CentralServer_onEvent;
                centralServer.ignoreCert = ignoreCert;
                centralServer.SetCertificateAuthority(argCAName, argCATemplate, argCertCommonName, argCertAltNames);
                centralServer.Start();
            }
            catch (Exception ex)
            {
                Log("Exception" + ex.ToString());
                throw ex;
            }
        }

        private void LocalPipeServer_onStateChange(bool connected)
        {
            lock (eventsList)
            {
                for (int i = eventsList.Count - 1; i >= 0; i--)
                {
                    MeshCentralSatelliteServer.ServerEvent e = eventsList[i];
                    localPipeServer.Send("event|" + e.icon + "|" + e.time.Ticks + "|" + e.msg);
                }
            }
        }

        private void LocalPipeServer_onMessage(string msg)
        {
            if (msg == "AddTestComputer")
            {
                try
                {
                    centralServer.AddTestComputer();
                }
                catch (Exception ex)
                {
                    CentralServer_onMessage(ex.Message.Replace("\r\n", ""));
                }
            }
            else if (msg == "RemoveTestComputer")
            {
                try
                {
                    centralServer.RemoveTestComputer();
                }
                catch (Exception ex)
                {
                    CentralServer_onMessage(ex.Message.Replace("\r\n", ""));
                }
            }
            else if (msg == "ListCertificateAuthorities")
            {
                try
                {
                    centralServer.EventCertificateAuthorities();
                }
                catch (Exception ex)
                {
                    CentralServer_onMessage(ex.Message.Replace("\r\n", ""));
                }
            }
        }

        private void CentralServer_onEvent(MeshCentralSatelliteServer.ServerEvent e)
        {
            lock (eventsList)
            {
                eventsList.Insert(0, e);
                while (eventsList.Count > 100) { eventsList.RemoveAt(99); }
            }
            localPipeServer.Send("event|" + e.icon + "|" + e.time.Ticks + "|" + e.msg);
        }

        private void CentralServer_onMessage(string msg)
        {
            localPipeServer.Send("log|" + msg);
            Log(msg);
        }

        private void CentralServer_onStateChanged(int state)
        {
            
        }

        private void StopServer(bool critical)
        {
            if (centralServer == null) return;
            centralServer.onStateChanged -= CentralServer_onStateChanged;
            centralServer.onMessage -= CentralServer_onMessage;
            centralServer.onEvent -= CentralServer_onEvent;
            centralServer.Stop();
            centralServer.Dispose();
            centralServer = null;
        }

        private void centralServer_OnSignal(object sender, int signal)
        {
            Log("Service signal: " + signal);

            if (signal == 2 || signal == 3) { StopServer(false); base.OnStop(); }
            if (signal == 1)
            {
                StopServer(false);
                ServerLaunchThread = new Thread(new ThreadStart(AttemptServerLaunch));
                ServerLaunchThread.IsBackground = true;
                ServerLaunchThread.Start();
            }
        }

        // Got a fatal error from the server. Stop and attempt to restart.
        private object stopLock = new object();
        private void centralServer_OnFatalError(object sender, EventArgs e)
        {
            lock (stopLock)
            {
                Log("Critical error, shutting down and attempting restart.");
                StopServer(true);
                ServerLaunchThread = new Thread(new ThreadStart(AttemptServerLaunch));
                ServerLaunchThread.IsBackground = true;
                ServerLaunchThread.Start();
            }
        }

        protected override void OnStop()
        {
            Log("Service stopped");

            // Stop and clean up server
            StopServer(false);

            base.OnStop();

            if (ServerLaunchThread != null)
            {
                ServerLaunchThread.Abort();
                ServerLaunchThread = null;
            }
        }


        public static void ExceptionSink(object sender, System.Threading.ThreadExceptionEventArgs args)
        {
            Log("Exception: " + args.Exception.ToString());
        }

        public static void UnhandledExceptionEventSink(object sender, UnhandledExceptionEventArgs args)
        {
            Log("Exception: " + ((Exception)args.ExceptionObject).ToString());
        }

        public static void Log(string msg)
        {
            if (log) { try { File.AppendAllText(logFilePath, DateTime.Now.ToString("HH:mm:tt.ffff") + ": " + msg + "\r\n"); } catch (Exception) { } }
            if (debug) { try { File.AppendAllText(debugFilePath, DateTime.Now.ToString("HH:mm:tt.ffff") + ": " + msg + "\r\n"); } catch (Exception) { } }
        }

    }
}
