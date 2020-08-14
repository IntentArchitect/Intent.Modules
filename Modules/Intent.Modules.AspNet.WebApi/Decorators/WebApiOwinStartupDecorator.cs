using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class WebApiOwinStartupDecorator : IOwinStartupDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.AspNet.WebApi.OwinStartup.Decorator";

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "System.Web.Http",
            };
        }

        public IEnumerable<string> Configuration()
        {
            return new []
            {
                "WebApiConfig.Configure(app);",
            };
        }

        public IEnumerable<string> Methods()
        {
            return new string[0];
        }

        public int Priority { get; set; } = -100;

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(OwinWebApiConfigTemplate.Identifier),
            };
        }
    }
}