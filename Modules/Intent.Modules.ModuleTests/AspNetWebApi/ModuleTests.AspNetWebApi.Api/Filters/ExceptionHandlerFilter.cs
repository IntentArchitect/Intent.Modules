using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.AspNet.WebApi.ExceptionHandlerFilter", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Api.Filters
{
    public class ExceptionHandlerFilter : ExceptionFilterAttribute
    {
        [IntentManaged(Mode.Ignore)]
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // Customize your response messages by inspecting 'actionExecutedContext.Exception'
            // for specific Exception types.
            // e.g.
            // 
            // if (actionExecutedContext.Exception is ArgumentException)
            // {
            //	actionExecutedContext.Response = new HttpResponseMessage
            //	{
            //		StatusCode = HttpStatusCode.BadRequest,
            //		Content = new StringContent(@"{ ""message"": ""You have specified an invalid argument"" }"),
            //		ReasonPhrase = "Invalid input supplied for service operation"
            //	};
            //	return;
            // }

            // Generic return message. Use this to mask internal errors so that consumers of your
            // service cannot see what the technical details are.

#if DEBUG
			// This is purely for Developer convenience to get started. Please note that
			// this is not the standard way for deploying code into production.
			actionExecutedContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent($@"{{ ""message"": ""{actionExecutedContext.Exception.Message}"" }}"),
                ReasonPhrase = "An internal server error occurred"
            };
#else
            actionExecutedContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent(@"{ ""message"": ""An internal server error occurred"" }"),
                ReasonPhrase = "An internal server error occurred"
            };
#endif
        }
    }
}