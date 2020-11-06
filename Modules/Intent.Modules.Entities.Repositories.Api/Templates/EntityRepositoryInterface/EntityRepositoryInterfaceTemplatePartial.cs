using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface
{
    partial class EntityRepositoryInterfaceTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.Entities.Repositories.Api.EntityInterface";

        public EntityRepositoryInterfaceTemplate(ClassModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.Identifier);

        public string EntityStateName => GetTypeName(GetMetadata().CustomMetadata["Entity Template Id"], Model);

        public string EntityInterfaceName => GetTypeName(GetMetadata().CustomMetadata["Entity Interface Template Id"], Model); 

        public string PrimaryKeyType => Types.Get(Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Type)?.Name ?? "Guid";

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}Repository",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
