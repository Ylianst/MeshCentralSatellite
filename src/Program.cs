﻿/*
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
using System.Text;
using System.Windows.Forms;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace MeshCentralSatellite
{
    static class Program
    {
        public static bool ServiceLaunchAttempt = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Run as a windows service
            if (!System.Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new MeshSatelliteService() };
                ServiceBase.Run(ServicesToRun);
                if (ServiceLaunchAttempt) return; // This is a bit of a hack, we don't really know if we are launching as a a service so we try it.
            }

            // Check is we need to install/uninstall the service
            string parameter = string.Concat(args).ToLower();
            switch (parameter)
            {
                case "/install":
                case "-install":
                case "--install":
                    {
                        if (!IsAdministrator()) { MessageBox.Show("Must be administrator to perform this operation", "Service Installer"); return; }
                        try { ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location }); }
                        catch (Exception ex) { MessageBox.Show(ex.ToString(), "Service Installer"); }
                        return;
                    }
                case "/uninstall":
                case "-uninstall":
                case "--uninstall":
                    {
                        if (!IsAdministrator()) { MessageBox.Show("Must be administrator to perform this operation", "Service UnInstaller"); return; }
                        try { ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location }); }
                        catch (Exception ex) { MessageBox.Show(ex.ToString(), "Service UnInstaller"); }
                        return;
                    }
            }

            // Setup settings & visual style
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Properties.Settings.Default.Upgrade();

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ExceptionSink);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionEventSink);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);

            MainForm main;
            System.Globalization.CultureInfo currentCulture;
            do
            {
                currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                main = new MainForm(args);
                if (main.forceExit == false) { Application.Run(main); }
            }
            while (currentCulture.Equals(System.Threading.Thread.CurrentThread.CurrentUICulture) == false);
        }

        public static void Debug(string msg) { try { File.AppendAllText("debug.log", msg + "\r\n"); } catch (Exception) { } }

        public static void ExceptionSink(object sender, System.Threading.ThreadExceptionEventArgs args)
        {
            Debug("ExceptionSink: " + args.Exception.ToString());
        }

        public static void UnhandledExceptionEventSink(object sender, UnhandledExceptionEventArgs args)
        {
            Debug("UnhandledExceptionEventSink: " + ((Exception)args.ExceptionObject).ToString());
        }

        public static bool IsAdministrator()
        {
            return ((new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)));
        }
    }
}
