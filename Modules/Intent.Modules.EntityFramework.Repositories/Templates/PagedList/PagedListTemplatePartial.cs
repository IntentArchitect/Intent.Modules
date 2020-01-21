using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.PagedList
{
    partial class PagedListTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.PagedList";

        public PagedListTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        public string PagedResultInterfaceName => GetTemplateClassName(PagedResultInterfaceTemplate.Identifier);

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
