using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;

namespace JwtWebApiSelfHost.Filter
{
    /// <summary>
    /// Validate the model given in request body and passed as a parameter in a controller
    /// </summary>
    public sealed class ModelValidatorFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Get model validation result and response to client
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
                return;

            if (actionContext.ModelState.IsValid)
            {
                //Valid model, pass to the next filter
                base.OnActionExecuting(actionContext);
            }
            else  //Handle invalid model
            {
                //Capture all model exception messages
                List<Exception> exceptions = new List<Exception>();
                foreach (KeyValuePair<string, System.Web.Http.ModelBinding.ModelState> state in actionContext.ModelState)
                {
                    if (state.Value.Errors.Count != 0)
                    {
                        exceptions.AddRange(state.Value.Errors.Select(error => error.Exception));
                    }
                }

                if (exceptions.Count > 0)
                {
                    //Generate exception messages
                    StringBuilder exceptionMessages = new StringBuilder();
                    int exceptionMessageIndex = 1;

                    exceptions.ForEach(e => {
                        exceptionMessages.Append($"Number of total exception:{exceptions.Count}. Exception messages:[{exceptionMessageIndex++}]{e.Message}");
                    });

                    //Create response
                    actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest, new Utility.ResultMessage()
                    {
                        Result = "Invalid parameter structure",
                        Details = exceptionMessages.ToString()
                    },
                    actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
                }
            }
        }
    }
}
