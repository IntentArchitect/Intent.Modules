using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.Modules.AspNet.WebApi.Templates.RequireHttpsMiddleware;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class UseHttpsOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.AspNet.WebApi.OwinStartup.UseHttpsDecorator";

        public UseHttpsOwinStartupDecorator()
        {
        }
        public IEnumerable<string> DeclareUsings()
        {
            return new[]
                {
                    "NWebsec.Owin",
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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(RequireHttpsMiddlewareTemplate.Identifier),
            };
        }
    }
}