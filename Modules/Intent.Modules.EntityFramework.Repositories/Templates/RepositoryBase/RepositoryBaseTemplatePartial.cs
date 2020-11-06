using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFramework.Repositories.Templates.PagedList;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.RepositoryBase
{
    partial class RepositoryBaseTemplate : CSharpTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.BaseRepository";

        public RepositoryBaseTemplate(IProject project)
            : base(Identifier, project)
        {
            AddNugetDependency(new NugetPackageInfo("EntityFramework", "6.2.0"));
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.Identifier);
        public string PagedListClassName => GetTypeName(PagedListTemplate.Identifier);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"RepositoryBase",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
