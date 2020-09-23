using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using Newtonsoft.Json.Linq;

namespace JwtWebApiSelfHost.Utility
{
    /// <summary>
    /// Generate HttpResponseMessages in JSON format
    /// </summary>
    public class ResponseResult
    {
        /// <summary>
        /// Message templete for most api results.
        /// </summary>
        public struct ResultMessage
        {
            public object Result { get; set; }
            public string Details { get; set; }
        }

        /// <summary>
        /// Generate response message while the return value is a single, primitive value
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static HttpResponseMessage ResponseSinglePrimitiveValue(HttpStatusCode code, ValueType value, string details)
        {
            ResultMessage result = new ResultMessage()
            {
                Result = value,
                Details = details
            };
            return new HttpResponseMessage(code)
            {
                Content = new JsonContent(JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8)
            };
        }

        /// <summary>
        /// 參數不合法
        /// </summary>
        public static HttpResponseMessage InvalidParameters(string details)
        {
            ResultMessage result = new ResultMessage()
            {
                Result = "Invalid Parameters",
                Details = details
            };

            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new JsonContent(JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8)
            };
        }
    }

    /// <summary>
    /// Generate Response content in JSON format
    /// </summary>
    public class JsonContent : StringContent
    {
        public JsonContent(string content) : this(content, Encoding.UTF8)
        {   }

        public JsonContent(string content, Encoding encoding) : base(content, encoding, "application/json")
        {   }
    }
}
