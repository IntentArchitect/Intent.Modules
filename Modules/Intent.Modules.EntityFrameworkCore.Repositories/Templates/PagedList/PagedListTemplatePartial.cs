using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList
{
    partial class PagedListTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFrameworkCore.Repositories.BaseRepository";

        public PagedListTemplate(IProject project)
            : base(Identifier, project)
        {
            AddNugetDependency(new NugetPackageInfo("Intent.Framework.Core", "2.1.1"));
            AddNugetDependency(new NugetPackageInfo("Intent.Framework.Domain", "2.1.1"));
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"PagedList",
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: "PagedList",
                @namespace: "${Project.Name}"
                );
        }
    }
}
