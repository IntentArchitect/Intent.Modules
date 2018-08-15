using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity.Decorators
{
    public class UnityWebApiConfigTemplateDecorator : IWebApiConfigTemplateDecorator, IHasNugetDependencies, IDeclareUsings
    {
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
            return new[] { "Unity.WebApi" };
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
