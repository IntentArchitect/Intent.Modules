using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    partial class ServiceImplementationTemplate : CSharpTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, ITemplateBeforeExecutionHook, IHasDecorators<ServiceImplementationDecoratorBase>, ITemplatePostCreationHook
    {
        private IList<ServiceImplementationDecoratorBase> _decorators = new List<ServiceImplementationDecoratorBase>();

        public const string Identifier = "Intent.Application.ServiceImplementations";
        public ServiceImplementationTemplate(IProject project, ServiceModel model)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            Types.AddClassTypeSource(CSharpTypeSource.Create(ExecutionContext, DTOTemplate.IDENTIFIER, "List<{0}>"));
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.IDENTIFIER, x => x.Id == Model.Id)
            }.Union(GetDecorators().SelectMany(x => x.GetConstructorDependencies(Model).Select(d => d.TemplateDependency)));
        }

        public void AddDecorator(ServiceImplementationDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<ServiceImplementationDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", GetServiceInterfaceName()},
                { "ConcreteType", $"{Namespace}.{ClassName}" },
                { "InterfaceTypeTemplateId", ServiceContractTemplate.IDENTIFIER },
                { "ConcreteTypeTemplateId", Identifier }
            });
        }

        public override string DependencyUsings
        {
            get
            {
                var builder = new StringBuilder(base.DependencyUsings).AppendLine();
                var additionalUsings = GetDecorators()
                    .SelectMany(s => s.GetUsings(Model))
                    .Distinct()
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToArray();
                foreach (var @using in additionalUsings)
                {
                    builder.AppendLine($"using {@using};");
                }

                return builder.ToString();
            }
        }

        private string GetOperationDefinitionParameters(OperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationCallParameters(OperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.TypeReference)}>" : GetTypeName(o.TypeReference);
        }

        public string GetServiceInterfaceName()
        {
            var serviceContractTemplate = Project.Application.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.IDENTIFIER, x => x.Id == Model.Id));
            return $"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}";
        }

        private IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            var parameters = GetDecorators()
                .SelectMany(s => s.GetConstructorDependencies(this.Model))
                .Distinct()
                .ToArray();
            return parameters;
        }

        private string GetImplementation(OperationModel operation)
        {
            var output = GetDecorators().Aggregate(x => x.GetDecoratedImplementation(Model, operation));
            if (string.IsNullOrWhiteSpace(output))
            {
                return @"
            throw new NotImplementedException(""Write your implementation for this service here..."");";
            }
            return output;
        }
    }
}
