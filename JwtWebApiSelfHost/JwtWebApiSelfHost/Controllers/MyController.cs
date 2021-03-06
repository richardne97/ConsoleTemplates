﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using JwtWebApiSelfHost.Model;
using Microsoft.Owin.Security.Provider;

namespace JwtWebApiSelfHost.Controllers
{
    /// <summary>
    /// Controllers
    /// </summary>
    [Authorize]
    [RoutePrefix("My")]
    public class MyController : ApiController
    {
        private readonly object _injectObject;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="injectObject"></param>
        public MyController(object injectObject)
        {
            _injectObject = injectObject;
        }

        /// <summary>
        /// MyGetAction
        /// </summary>
        /// <param name="message">Request message in get parameter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public string MyGetAction([FromUri] string message)
        {
            return $"{User.Identity.Name}, I got your message. '{message}'";
        }

        /// <summary>
        /// MyPostAction
        /// </summary>
        /// <param name="requestModel">model in post body (JSON format)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ResponseModel MyPostAction([FromBody]RequestModel requestModel)
        {
            return new ResponseModel()
            {
                Message = requestModel.Message
            };
        }
    }
}
