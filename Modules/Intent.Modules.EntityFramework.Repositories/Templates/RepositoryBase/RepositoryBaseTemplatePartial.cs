using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFramework.Repositories.Templates.PagedList;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.RepositoryBase
{
    partial class RepositoryBaseTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.BaseRepository";

        public RepositoryBaseTemplate(IProject project)
            : base(Identifier, project)
        {
            AddNugetDependency(new NugetPackageInfo("EntityFramework", "6.2.0"));
        }

        public string RepositoryInterfaceName => GetTemplateClassName(RepositoryInterfaceTemplate.Identifier);
        public string PagedListClassName => GetTemplateClassName(PagedListTemplate.Identifier);

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"RepositoryBase",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "RepositoryBase",
                @namespace: "${Project.Name}"
                );
        }
    }
}
