using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;

namespace JwtWebApiSelfHost.Filter
{
    /// <summary>
    /// Validate the model given in request body and passed as a parameter in a controller
    /// </summary>
    public class ModelValidatorFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                var exceptions = new List<Exception>();
                foreach (var state in actionContext.ModelState)
                {
                    if (state.Value.Errors.Count != 0)
                    {
                        exceptions.AddRange(state.Value.Errors.Select(error => error.Exception));
                    }
                }

                if (exceptions.Count > 0)
                {
                    StringBuilder exceptionMessages = new StringBuilder();
                    int exceptionMessageIndex = 1;
                    exceptions.ForEach(e => {
                        exceptionMessages.Append($"Number of total exception:{exceptions.Count()}. Exception messages:[{exceptionMessageIndex++}]{e.Message}");
                    });

                    actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest, new Utility.ResponseResult.ResultMessage()
                    {
                        Result = "Invalid Parameter",
                        Details = exceptionMessages.ToString()
                    },
                    actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
                }
            }
        }
    }
}
