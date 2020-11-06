using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    partial class RepositoryInterfaceTemplate : CSharpTemplateBase, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.Entities.Repositories.Api.BaseInterface";

        public RepositoryInterfaceTemplate(IProject project)
            : base(Identifier, project)
        {
        }
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IRepository",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
