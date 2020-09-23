using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using JwtWebApiSelfHost.Model;
using Microsoft.Owin.Security.Provider;

namespace JwtWebApiSelfHost.Controllers
{
    [Authorize]
    [RoutePrefix("My")]
    public class MyController
    {
        /// <summary>
        /// MyGetAction
        /// </summary>
        /// <param name="message">Request message in get parameter</param>
        /// <returns></returns>
        [HttpGet]
        public string MyGetAction([FromUri] string message)
        {
            return message;
        }

        /// <summary>
        /// MyPostAction
        /// </summary>
        /// <param name="requestModel">model in post body (JSON format)</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseModel MyPostAction([FromBody]RequestModel requestModel)
        {
            return new ResponseModel()
            {
                Message = requestModel.Message
            };
        }
    }
}
