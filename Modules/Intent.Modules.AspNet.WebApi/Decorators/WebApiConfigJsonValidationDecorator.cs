using System.Collections.Generic;
using Intent.Modules.WebApi.Templates.OwinWebApiConfig;

namespace Intent.SoftwareFactory.Modules.Decorators.WebApi
{
    public class WebApiConfigJsonValidationDecorator : IWebApiConfigTemplateDecorator
    {
        public const string Identifier = "Intent.WebApi.WebApiConfig.JsonValidation.Decorator";

        public IEnumerable<string> Configure()
        {
            return new[]
            {
                "// As per http://www.asp.net/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api",
                "config.Filters.Add(new ValidateModelActionFilter());",
                "config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;",
            };
        }
    }
}
