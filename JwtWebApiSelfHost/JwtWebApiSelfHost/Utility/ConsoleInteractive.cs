using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace JwtWebApiSelfHost.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConsoleInteractive
    {
        #region DisableQuick Edit

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        /// <summary>
        /// Get std handle
        /// </summary>
        /// <param name="nStdHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// Get console mode
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="lpMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        /// <summary>
        /// Set console mode
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="dwMode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #endregion

        #region Disable ControlBox

        private const int MF_BYCOMMAND = 0x00000000;
        
        /// <summary>
        /// 
        /// </summary>
        public const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Delete menu
        /// </summary>
        /// <param name="hMenu"></param>
        /// <param name="nPosition"></param>
        /// <param name="wFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        /// <summary>
        /// Get System menu pointer
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="bRevert"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// Get Console Window pointer
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetConsoleWindow();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isReboot"></param>
        /// <param name="e"></param>
        public static void LogAndRestartUnHandledException(bool isReboot, UnhandledExceptionEventArgs e)
        {
            //Write unhandle exception message to Fatal log
            StringBuilder sb = new StringBuilder("<UnhandledException>");
            if (e.ExceptionObject != null)
            {
                Type objType = e.ExceptionObject.GetType();
                sb.Append($"ObjectType: {objType.FullName} Exception:{e.ExceptionObject}\r\n");
            }
            Trace.Write(sb.ToString(), "Fatal");

            if (isReboot)
            {
                //Halt for 10 sec before restart
                Thread.Sleep(10000);
                
                //Restart program
                using (Process newProcess = new Process())
                {
                    newProcess.StartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location);
                    newProcess.Start();
                };
            }
            
            //Give code 1, to prevent Windows OS shows "Program stop working" message window and ask user to response.
            Environment.Exit(1);
        }

        /// <summary>
        /// Disable control box close button
        /// </summary>
        public static bool DisableControlBoxCloseButton()
        {
            if (DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Disable consonle quick edit, in case pausing console program by accident
        /// </summary>
        /// <returns></returns>
        public static bool DisableQuickEdit()
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            if (!GetConsoleMode(consoleHandle, out uint consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (SetConsoleMode(consoleHandle, consoleMode))
                return true;
            else
            {
                // ERROR: Unable to set console mode
                return false;
            }
        }
    }
}
