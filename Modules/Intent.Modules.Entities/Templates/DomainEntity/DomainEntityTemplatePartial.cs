using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    partial class DomainEntityTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasDecorators<DomainEntityDecoratorBase>, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.Entities.DomainEntity";
        private readonly IList<DomainEntityDecoratorBase> _decorators = new List<DomainEntityDecoratorBase>();

        public DomainEntityTemplate(ClassModel model, IOutputTarget project)
            : base(Identifier, project, model)
        {
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityInterfaceTemplate.Identifier));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public void AddDecorator(DomainEntityDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string Constructors(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

        public string GetParametersDefinition(OperationModel operation)
        {
            return string.Join(", ", operation.Parameters.Select(x => $"{GetTypeName(x.Type)} {x.Name.ToCamelCase()}"));
            //return operation.Parameters.Any() 
            //    ? operation.Parameters.Select(x => GetTypeName(x.Type) + " " +).Aggregate((x, y) => x + ", " + y) 
            //    : "";
        }

        public string EmitOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.ReturnType);
        }
    }
}
