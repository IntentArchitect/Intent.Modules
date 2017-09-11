using System.Collections.Generic;
using System.Linq;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.WebApi.Templates.OwinWebApiConfig
{
    partial class OwinWebApiConfigTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasTemplateDependencies, IHasDecorators<IWebApiConfigTemplateDecorator>
    {
        public const string Identifier = "Intent.WebApi.OwinWebApiConfig";
        private IEnumerable<IWebApiConfigTemplateDecorator> _decorators;

        public OwinWebApiConfigTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public IEnumerable<string> ConfigureItems => GetDecorators().SelectMany(x => x.Configure().Union(new[] {string.Empty}));

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
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
                NugetPackages.UnityWebApi,
                NugetPackages.MicrosoftWebInfrastructure
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UnityConfigTemplate.Identifier) // TODO: We should not need to lock ourselves into Unity (or Owin for that matter) for WebApi
            };
        }

        public IEnumerable<IWebApiConfigTemplateDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }
    }
}
