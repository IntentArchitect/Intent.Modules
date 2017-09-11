using System.Collections.Generic;
using Intent.Modules.WebApi.Templates.OwinWebApiConfig;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.WebApi.Decorators
{
    public class WebApiOwinStartupDecorator : IOwinStartupDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.WebApi.OwinStartup";

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using System.Web.Http;",
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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(OwinWebApiConfigTemplate.Identifier),
            };
        }
    }
}