using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ServiceModel.Security;

namespace JwtWebApiSelfHost.Utility
{
    /// <summary>
    /// Config http.sys 
    /// </summary>
    public class HttpServerConfig
    {
        private string _listenUri = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listenUri">Example: http://+:8000/</param>
        public HttpServerConfig(string listenUri)
        {
            _listenUri = listenUri;
        }

        /// <summary>
        /// Add given url listen path in http.sys
        /// </summary>
        /// <returns></returns>
        public bool DeleteUrlAcl(string user = "Everyone")
        {
            Process ps = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    Verb = "runas",
                    CreateNoWindow = false,
                    FileName = "netsh",
                    Arguments = $"http delete urlacl url={_listenUri}",
                    RedirectStandardOutput = false,
                    UseShellExecute = true
                }
            };
            ps.Start();
            ps.WaitForExit();

            if (IsEnable)
                return true;
            else
            {
                Trace.WriteLine($"Delete uralacl to http.sys fail.");
                return false;
            }
        }

        /// <summary>
        /// Add given url listen path in http.sys
        /// </summary>
        /// <returns></returns>
        public bool AddUrlAcl(string user = "Everyone")
        {
            Process ps = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    Verb = "runas",
                    CreateNoWindow = false,
                    FileName = "netsh",
                    Arguments = $"http add urlacl url={_listenUri} User={user}",
                    RedirectStandardOutput = false,
                    UseShellExecute = true
                }
            };
            ps.Start();
            ps.WaitForExit();

            if (IsEnable)
                return true;
            else
            {
                Trace.WriteLine($"Add uralacl to http.sys fail.");
                return false;
            }
        }

        /// <summary>
        /// Check if the given Uri has configured in http.sys 
        /// </summary>
        /// <returns></returns>
        public bool IsEnable
        {
            get
            {
                Process ps = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        FileName = "netsh",
                        Arguments = $"http show urlacl url={_listenUri}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                StringBuilder sb = new StringBuilder();
                ps.Start();
                while (!ps.StandardOutput.EndOfStream)
                    sb.Append(ps.StandardOutput.ReadToEnd());
                ps.WaitForExit();

                if (sb.ToString().Contains("Listen: Yes") || sb.ToString().Contains("接聽: Yes"))
                    return true;
                else
                    return false;
            }
        }
    }
}
