using System.Web.Http;
using Intent.RoslynWeaver.Attributes;
using ModuleTests.AspNetWebApi.Api.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.AspNet.WebApi.OwinWebApiConfig", Version = "1.0")]

namespace ModuleTests.AspNetWebApi.Api
{
    public static class WebApiConfig
    {
        [IntentManaged(Mode.Fully)]
        public static void Configure(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Adds "Z" to the end of serialized DateTime, so that clients are aware that the received time is UTC
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            AddCustomConfiguration(config);

            config.Filters.Add(new ExceptionHandlerFilter());

            app.UseWebApi(config);
        }

        [IntentManaged(Mode.Ignore)]
        public static void AddCustomConfiguration(HttpConfiguration config)
        {

        }


    }
}