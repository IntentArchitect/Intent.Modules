using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface
{
    partial class PagedResultInterfaceTemplate : CSharpTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Entities.Repositories.Api.PagedResultInterface";

        public PagedResultInterfaceTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IPagedResult",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
