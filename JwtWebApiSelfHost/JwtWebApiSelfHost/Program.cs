﻿using JwtWebApiSelfHost.Utility;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JwtWebApiSelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Properties.Settings.Default.UnhandledExceptionReboot)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            #region Redirect Trace messages to NLogTraceListen
            NLogTraceListener nLogTraceListener = new NLogTraceListener(true);
            Trace.Listeners.Add(nLogTraceListener);
            Trace.AutoFlush = true;
            #endregion

            //Display Program name and version while starts
            //顯示軟體版本
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Trace.WriteLine($"{assemblyName.Name} {assemblyName.Version} Starts");

            //Disable close button in control box.
            //使 Console 視窗關閉功能無法運作
            ConsoleInteractive.DisableControlBoxCloseButton();
            //Disable quick edit
            //使 Console 視窗輸入功能無法運作，避免誤觸導致系統停止運作
            ConsoleInteractive.DisableQuickEdit();
            
            #region Check http listen configuration and start listen 檢查並啟用 Http Web API 伺服器

            HttpServerConfig hsc = new HttpServerConfig(Properties.Settings.Default.ListenPort);
            string webListenUrl = $"http://+:{Properties.Settings.Default.ListenPort}/";
            bool isHttpSysEnabled = hsc.IsEnable;
            
            if (!isHttpSysEnabled)
            {
                if (hsc.AddUrlAcl())
                {
                    Trace.WriteLine($"Add new urlacl {webListenUrl} successfuly");
                    isHttpSysEnabled = true;
                }
                else
                    throw new Exception("Http listen can not be started.");
            }

            if (isHttpSysEnabled)
            {
                WebApp.Start<Startup>(webListenUrl);
                Trace.WriteLine($"Listen {webListenUrl}");
            }

            #endregion

            //Accept user console input and response
            string cmd = string.Empty;
            string helpContent = File.ReadAllText($"{AppContext.BaseDirectory}helpContent.txt");
            
            while (true)
            {
                Console.Write("\r\n>");
                if (string.IsNullOrEmpty(cmd))
                    Console.Write(helpContent);
                else if (cmd == "quit")
                    break;
                else
                    CommandHandler(cmd);
            }

            #region Dispose resouces before leave




            #endregion
        }

        static void CommandHandler(string cmd)
        {
            if (cmd != null && cmd.Any())
            {
                switch (cmd)
                {
                    case "Commmand1":
                        break;

                    case "Commmand2":
                        break;
                }
            }
        }

        /// <summary>
        /// Capture global unhandled exception event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Write unhandle exception message to Fatal log
            StringBuilder sb = new StringBuilder();
            sb.Append("UnhandledException");
            if (e.ExceptionObject != null)
            {
                Type objType = e.ExceptionObject.GetType();
                sb.Append($"Object: {objType} Type: {objType.Name}\r\n");
            }
            sb.Append($"{e.ExceptionObject}\r\n");
            Trace.Write(sb.ToString(), "Fatal");

            //Restart program
            using (Process newProcess = new Process())
            {
                newProcess.StartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                newProcess.Start();
                //Give code 1, to prevent Windows OS shows "Program stop working" message window and ask user to response.
                Environment.Exit(1);
            };
        }
    }
}
