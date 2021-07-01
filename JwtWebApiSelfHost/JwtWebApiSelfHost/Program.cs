using JwtWebApiSelfHost.Utility;
using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.IO;

namespace JwtWebApiSelfHost
{
    class Program
    {
        static void Main()
        {
            #region Console Interactive setting
            
            //Capture unhandled exception
            AppDomain.CurrentDomain.UnhandledException += (o,e) =>
            {
                ConsoleInteractive.LogAndRestartUnHandledException(Properties.Settings.Default.UnhandledExceptionReboot, e);
            };

            //Disable close button in control box.
            //使 Console 視窗關閉功能無法運作
            ConsoleInteractive.DisableControlBoxCloseButton();
            //Disable quick edit
            //使 Console 視窗輸入功能無法運作，避免誤觸導致系統停止運作
            ConsoleInteractive.DisableQuickEdit();
            #endregion

            #region Redirect Trace messages to NLogTraceListen
            NLogTraceListener nLogTraceListener = new NLogTraceListener(true);
            Trace.Listeners.Add(nLogTraceListener);
            Trace.AutoFlush = true;
            #endregion

            #region Display start up message
            //Display Program name and version while starts
            //顯示軟體版本
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Trace.WriteLine($"{assemblyName.Name} {assemblyName.Version} Starts");
            #endregion

            #region Check http listen configuration and start listen 檢查並啟用 Http Web API 伺服器

            HttpServerConfig hsc = new HttpServerConfig(Properties.Settings.Default.ListenPort, HttpServerConfig.HttpListenTypes.http);
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
                    throw new UriFormatException("Http listen can not be started.");
            }

            if (isHttpSysEnabled)
            {
                var startOptions = new StartOptions();
                startOptions.Urls.Add(webListenUrl);
                //startOptions.Urls.Add("https://+:443");  //Enable Https

                //Can not listem mutiple urls
                WebApp.Start<Startup>(startOptions);
                Trace.Listeners.Remove("HostingTraceListener");
                Trace.WriteLine($"Listen {webListenUrl}");
            }

            #endregion

            #region Handle Console interactive command 

            string helpContent = File.ReadAllText($"{AppContext.BaseDirectory}helpContent.txt");
            
            while (true)
            {
                Console.Write("\r\n>");
                
                string cmd = Console.ReadLine();
                if (string.IsNullOrEmpty(cmd))
                {
                    Console.Write(helpContent);
                }
                else if (cmd == "q")
                {
                    break;
                }
                else
                    CommandHandler(cmd);
            }
            #endregion

            #region Dispose resouces before leave (add your ocde here)




            #endregion
        }

        #region Handle console interactive command (add your code here) 

        //Remember to update helpContent.txt file. 

        /// <summary>
        /// Handle customized command here
        /// </summary>
        /// <param name="cmd"></param>
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

        #endregion
    }
}
