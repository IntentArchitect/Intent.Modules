using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class WebApiConfigJsonValidationDecorator : WebApiConfigTemplateDecoratorBase
    {
        public const string Identifier = "Intent.AspNet.WebApi.WebApiConfig.JsonValidation.Decorator";

        public override IEnumerable<string> Configure()
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
