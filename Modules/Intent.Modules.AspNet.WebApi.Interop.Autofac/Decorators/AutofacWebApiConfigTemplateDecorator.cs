using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Templates;
using System;
using System.Collections.Generic;
using Intent.Modules.Autofac.Templates.AutofacConfig;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AspNet.WebApi.Interop.Autofac.Decorators
{
    public class AutofacWebApiConfigTemplateDecorator : WebApiConfigTemplateDecoratorBase, IHasNugetDependencies, IDeclareUsings
    {
        private readonly IApplication _application;

        public AutofacWebApiConfigTemplateDecorator(IApplication application)
        {
            _application = application;
        }

        public const string Id = "Intent.AspNet.WebApi.Interop.Autofac.WebApiConfigDecorator";

        public override IEnumerable<string> Configure()
        {
            return new[]
            {
                "// Autofac",
                "config.DependencyResolver = new AutofacWebApiDependencyResolver(AutofacConfig.GetConfiguredContainer());",
                "app.UseAutofacMiddleware(AutofacConfig.GetConfiguredContainer());",
                "app.UseAutofacWebApi(config);",
        };
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Autofac.Integration.WebApi",
                _application.FindTemplateInstance<IClassProvider>(AutofacConfigTemplate.Identifier).Namespace
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] 
            {
                NugetPackages.AutofacOwin,
                NugetPackages.AutofacWebApi2,
                NugetPackages.AutofacWebApi2Owin
            };
        }
    }
}
