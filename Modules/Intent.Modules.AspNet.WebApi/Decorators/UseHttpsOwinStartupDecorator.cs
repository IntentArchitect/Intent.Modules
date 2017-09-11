using System.Collections.Generic;
using Intent.Modules.WebApi.Templates.RequireHttpsMiddleware;
using Intent.Packages.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.WebApi.Decorators
{
    public class UseHttpsOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.WebApi.OwinStartup.UseHttpsDecorator";

        public UseHttpsOwinStartupDecorator()
        {
        }
        public IEnumerable<string> DeclareUsings()
        {
            return new[]
                {
                    "using NWebsec.Owin;",
                };
        }

        public IEnumerable<string> Configuration()
        {
            return new[]
            {
                "#if !DEBUG",
                "app.UseHsts(options => options.MaxAge(days: 30));",
                "app.Use<RequireHttpsMiddleware>();",
                "#endif",
            };
        }

        public IEnumerable<string> Methods()
        {
            return new string[0];
        }

        public int Priority { get; set; } = -150;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NWebsecOwin,
            };
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(RequireHttpsMiddlewareTemplate.Identifier),
            };
        }
    }
}