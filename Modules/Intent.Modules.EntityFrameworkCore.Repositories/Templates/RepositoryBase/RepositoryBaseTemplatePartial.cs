using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase
{
    partial class RepositoryBaseTemplate : CSharpTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFrameworkCore.Repositories.BaseRepository";

        public RepositoryBaseTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        public string RepositoryInterfaceName => GetTemplateClassName(RepositoryInterfaceTemplate.Identifier);
        public string PagedListClassName => GetTemplateClassName(PagedListTemplate.Identifier);

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"RepositoryBase",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
