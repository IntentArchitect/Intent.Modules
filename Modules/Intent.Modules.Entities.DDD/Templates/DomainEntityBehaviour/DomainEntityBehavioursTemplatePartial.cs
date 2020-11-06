using System.Collections;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    partial class DomainEntityBehavioursTemplate : CSharpTemplateBase<ClassModel>, ITemplate, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.Entities.DDD.BehavioursInterface";

        public DomainEntityBehavioursTemplate(ClassModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            Types.AddClassTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityInterfaceTemplate.Identifier));
        }

        public string ClassStateName => Model.Name;

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}Behaviours",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        private string GetParametersDefinition(OperationModel operation)
        {
            return operation.Parameters.Any()
                ? operation.Parameters.Select(x => this.GetTypeName(x.Type) + " " + x.Name.ToCamelCase()).Aggregate((x, y) => x + ", " + y)
                : "";
        }

        public string EmitOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{this.GetTypeName(o.ReturnType)}>" : this.GetTypeName(o.ReturnType);
        }
    }
}
