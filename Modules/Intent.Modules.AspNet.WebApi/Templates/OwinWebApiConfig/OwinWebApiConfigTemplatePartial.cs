using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig
{
    partial class OwinWebApiConfigTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<WebApiConfigTemplateDecoratorBase>
    {
        public const string Identifier = "Intent.AspNet.WebApi.OwinWebApiConfig";
        private readonly IList<WebApiConfigTemplateDecoratorBase> _decorators = new List<WebApiConfigTemplateDecoratorBase>();

        public OwinWebApiConfigTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }
        public IEnumerable<string> ConfigureItems => GetDecorators().SelectMany(x => x.Configure().Union(new[] {string.Empty}));
        public string Methods() => GetDecorators().Aggregate(x => x.Methods());

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"WebApiConfig",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftAspNetWebApi,
                NugetPackages.MicrosoftAspNetWebApiOwin,
                NugetPackages.MicrosoftWebInfrastructure
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void AddDecorator(WebApiConfigTemplateDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<WebApiConfigTemplateDecoratorBase> GetDecorators()
        {
            return _decorators;
        }
    }
}
