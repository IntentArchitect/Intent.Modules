using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList
{
    partial class PagedListTemplate : CSharpTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFrameworkCore.Repositories.PagedList";

        public PagedListTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        public string PagedResultInterfaceName => GetTemplateClassName(PagedResultInterfaceTemplate.Identifier);

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"PagedList",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
