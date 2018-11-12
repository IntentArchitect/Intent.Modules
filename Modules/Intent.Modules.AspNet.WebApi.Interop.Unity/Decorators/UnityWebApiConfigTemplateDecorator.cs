using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity.Decorators
{
    public class UnityWebApiConfigTemplateDecorator : IWebApiConfigTemplateDecorator, IHasNugetDependencies, IDeclareUsings
    {
        private readonly IApplication _application;

        public UnityWebApiConfigTemplateDecorator(IApplication application)
        {
            _application = application;
        }

        public const string Id = "Intent.AspNet.WebApi.Interop.Unity.DataContractDecorator";

        public IEnumerable<string> Configure()
        {
            return new[]
            {
                "// Unity",
                "config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());"
            };
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Unity.WebApi",
                _application.FindTemplateInstance<IHasClassDetails>(UnityConfigTemplate.Identifier).Namespace
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] 
            {
                NugetPackages.UnityWebApi
            };
        }
    }
}
