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
        private readonly int _listenPort = 0;
        private readonly HttpListenTypes _httpListenType;

        /// <summary>
        /// Http Listen Types
        /// </summary>
        public enum HttpListenTypes 
        { 
            /// <summary>
            /// Http
            /// </summary>
            http, 
            /// <summary>
            /// Https
            /// </summary>
            https 
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listenPort"></param>
        /// <param name="httpListenTypes"></param>
        public HttpServerConfig(int listenPort, HttpListenTypes httpListenTypes)
        {
            _listenPort = listenPort;
            _httpListenType = httpListenTypes;
        }

        /// <summary>
        /// delete url listen path in http.sys
        /// 刪除 url listen
        /// </summary>
        /// <returns></returns>
        public bool DeleteUrlAcl()
        {
            Process ps = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    Verb = "runas",
                    CreateNoWindow = false,
                    FileName = "netsh",
                    Arguments = $"http delete urlacl url={_httpListenType}://+:{_listenPort}/",
                    RedirectStandardOutput = false,
                    UseShellExecute = true
                }
            };
            ps.Start();
            ps.WaitForExit();
            ps.Dispose();

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
        /// 新增 url listen
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
                    Arguments = $"http add urlacl url={_httpListenType}://+:{_listenPort}/ User={user}",
                    RedirectStandardOutput = false,
                    UseShellExecute = true
                }
            };
            ps.Start();
            ps.WaitForExit();
            ps.Dispose();

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
        /// 檢查 http listen 是否已經啟用
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
                        Arguments = $"http show urlacl url={_httpListenType}://+:{_listenPort}/",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                StringBuilder sb = new StringBuilder();
                ps.Start();
                while (!ps.StandardOutput.EndOfStream)
                    sb.Append(ps.StandardOutput.ReadToEnd());

                ps.WaitForExit();
                ps.Dispose();

                if (sb.ToString().Contains("Listen: Yes") || sb.ToString().Contains("接聽: Yes"))
                    return true;
                else
                    return false;
            }
        }
    }
}
