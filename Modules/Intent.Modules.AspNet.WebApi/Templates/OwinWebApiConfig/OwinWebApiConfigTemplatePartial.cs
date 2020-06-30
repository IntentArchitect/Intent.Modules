using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig
{
    partial class OwinWebApiConfigTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<WebApiConfigTemplateDecoratorBase>
    {
        public const string Identifier = "Intent.AspNet.WebApi.OwinWebApiConfig";
        private readonly IList<WebApiConfigTemplateDecoratorBase> _decorators = new List<WebApiConfigTemplateDecoratorBase>();

        public OwinWebApiConfigTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        public IEnumerable<string> ConfigureItems => GetDecorators().SelectMany(x => x.Configure().Union(new[] {string.Empty}));
        public string Methods() => GetDecorators().Aggregate(x => x.Methods());

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "WebApiConfig",
                fileExtension: "cs",
                defaultLocationInProject: "App_Start",
                className: "WebApiConfig",
                @namespace: "${Project.Name}"
                );
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
