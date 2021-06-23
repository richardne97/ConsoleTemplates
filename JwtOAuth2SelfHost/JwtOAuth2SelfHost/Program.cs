using SR15.GISPlatform.Utility;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JwtOAuth2SelfHost
{
    class Program
    {
        static bool _captureGlobalUnhandledExceptionAndRestartProgram = false;

        static void Main(string[] args)
        {
            if (_captureGlobalUnhandledExceptionAndRestartProgram)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            #region Redirect Trace messages to NLogTraceListen
            NLogTraceListener nLogTraceListener = new NLogTraceListener(true);
            Trace.Listeners.Add(nLogTraceListener);
            Trace.AutoFlush = true;
            #endregion

            //Display Program name and version while starts
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Trace.WriteLine($"{assemblyName.Name} {assemblyName.Version} Starts");

            //Disable close button in control box.
            Utility.ConsoleWindow.DisableCloseButton();

            //Start self-host Web API
            string _listenUri = $"http://+:{Properties.Settings.Default.ListenPort}/";
            HttpServerConfig hsc = new HttpServerConfig(_listenUri);

            if (hsc.IsEnable)
            {
                Trace.WriteLine($"Start selfhost web api. {_listenUri}");
                WebApp.Start<Startup>(_listenUri);
            }
            else
            {
                if (hsc.AddUrlAcl())
                {
                    Trace.WriteLine($"Add new urlacl {_listenUri} successfuly");
                    WebApp.Start<Startup>(_listenUri);
                }
                else
                    throw new Exception("Http listen can not be started.");
            }

            //Accept user console input and response
            List<char> commandInputBuufer = new List<char>();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cunKey = Console.ReadKey();
                    if (cunKey.Key == ConsoleKey.Enter)
                    {
                        string cmd = new string(commandInputBuufer.ToArray());
                        commandInputBuufer.Clear();
                        if (cmd == "q")
                        {
                            Console.WriteLine("Terminating...");
                            break;
                        }
                        else
                            CommandHandler(cmd);
                    }
                    else
                        commandInputBuufer.Add(cunKey.KeyChar);
                }
            }

            #region Dispose resouces

            //Add dispose resouce here before leave

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

                    default:
                        Console.WriteLine("Enter 'q' to exit");
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
            Process newProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location)
            };
            newProcess.Start();

            //Give code 1, to prevent Windows OS shows "Program stop working" message window and ask user to response.
            Environment.Exit(1);
        }
    }
}
