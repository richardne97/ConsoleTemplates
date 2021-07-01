using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NLog;
using System.Reflection;

namespace JwtWebApiSelfHost
{
    /// <summary>
    /// Customized trace listener for NLog
    /// </summary>
    public class NLogTraceListener : TraceListener
    {
        private readonly Logger _logger;
        private readonly bool _enableConsoleOutput = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enableConsoleOutput">Whether or not display trace message in Console as well</param>
        public NLogTraceListener(bool enableConsoleOutput)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _enableConsoleOutput = enableConsoleOutput;
        }

        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            string writeMsg = $"{GetCaller()}:\r\n{message}";
            _logger.Trace(writeMsg);

            if (_enableConsoleOutput)
                Console.WriteLine($"\r\n{DateTime.Now:yyyy-MM-ddTHH:mm:ss} {writeMsg}");
        }

        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            string writeMsg = $"{GetCaller()}:\r\n{message}";
            _logger.Trace(writeMsg);

            if (_enableConsoleOutput)
                Console.WriteLine($"\r\n{DateTime.Now:yyyy-MM-ddTHH:mm:ss} {writeMsg}");
        }

        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public override void Write(string message, string category)
        {
            string writeMsg = $"{GetCaller()}:\r\n{message}";
            if (_enableConsoleOutput)
                Console.WriteLine($"\r\n{DateTime.Now:yyyy-MM-ddTHH:mm:ss} {writeMsg}");
            
            WriteToNLog(writeMsg, category);
        }

        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public override void WriteLine(string message, string category)
        {
            string writeMsg = $"{GetCaller()}:\r\n{message}";
            if (_enableConsoleOutput)
                Console.WriteLine($"\r\n{DateTime.Now:yyyy/MM/dd HH:mm:ss} {writeMsg}");
            
            WriteToNLog(writeMsg, category);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetCaller()
        {
            StackFrame target;
            MethodBase mi;
            try
            {
                //取得前三層的 stack 名稱
                StackTrace stackTrace = new StackTrace(3, true);
                target = stackTrace.GetFrame(0);
                if (target == null)
                    return string.Empty;
                mi = target.GetMethod();
            }
            catch { return null; }
            return $"{mi.ReflectedType.Name}.{mi.Name}[{target.GetFileLineNumber()}]";
        }

        private void WriteToNLog(string message, string category)
        {
            switch (category)
            {
                case "Fatal":
                    _logger.Fatal(message);
                    break;
                case "Error":
                    _logger.Error(message);
                    break;
                case "Warn":
                    _logger.Warn(message);
                    break;
                case "Info":
                    _logger.Info(message);
                    break;
                case "Debug":
                    _logger.Debug(message);
                    break;
                case "Trace":
                default:
                    _logger.Trace(message);
                    break;
            }
        }
    }
}
