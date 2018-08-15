using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity.Decorators
{
    public class UnityWebApiConfigTemplateDecorator : IWebApiConfigTemplateDecorator, IHasNugetDependencies
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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] 
            {
                NugetPackages.UnityWebApi
            };
        }
    }
}
