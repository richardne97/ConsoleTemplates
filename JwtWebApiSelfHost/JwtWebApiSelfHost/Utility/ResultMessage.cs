using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtWebApiSelfHost.Utility
{
    /// <summary>
    /// Message templete for most api results.
    /// </summary>
    public struct ResultMessage
    {
        /// <summary>
        /// Response reulst object
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Details about the response
        /// </summary>
        public string Details { get; set; }
    }
}
