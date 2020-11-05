using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Templates;
using System;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AspNet.WebApi.Interop.Unity.Decorators
{
    public class UnityWebApiConfigTemplateDecorator : WebApiConfigTemplateDecoratorBase, IHasNugetDependencies, IDeclareUsings
    {
        private readonly IApplication _application;

        public UnityWebApiConfigTemplateDecorator(IApplication application)
        {
            _application = application;
        }

        public const string Id = "Intent.AspNet.WebApi.Interop.Unity.DataContractDecorator";

        public override IEnumerable<string> Configure()
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
                _application.FindTemplateInstance<IClassProvider>(UnityConfigTemplate.Identifier).Namespace
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
