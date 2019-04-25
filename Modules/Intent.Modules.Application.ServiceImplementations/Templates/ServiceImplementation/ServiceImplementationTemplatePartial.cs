using System;
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
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    partial class ServiceImplementationTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasTemplateDependencies, IBeforeTemplateExecutionHook, IHasDecorators<ServiceImplementationDecoratorBase>, IPostTemplateCreation
    {
        private IEnumerable<ServiceImplementationDecoratorBase> _decorators;

        public const string Identifier = "Intent.Application.ServiceImplementations";
        public ServiceImplementationTemplate(IProject project, IServiceModel model)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            Types.AddClassTypeSource(ClassTypeSource.InProject(Project, DTOTemplate.IDENTIFIER, "List"));
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.IDENTIFIER, x => x.Id == Model.Id)
            }.Union(GetDecorators().SelectMany(x => x.GetConstructorDependencies(Model).Select(d => d.TemplateDependency)));
        }

        public IEnumerable<ServiceImplementationDecoratorBase> GetDecorators()
        {
            if (_decorators == null)
            {
                _decorators = Project.ResolveDecorators(this);
            }
            return _decorators;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "ServiceImplementation",
                className: "${Model.Name}",
                @namespace: "${Project.ProjectName}.ServiceImplementation"

                );
        }

        public void BeforeTemplateExecution()
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

        private string GetOperationDefinitionParameters(IOperation o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.Type)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationCallParameters(IOperation o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(IOperation o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.ReturnType.Type)}>" : GetTypeName(o.ReturnType.Type);
        }

        public string GetServiceInterfaceName()
        {
            var serviceContractTemplate = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.IDENTIFIER, x => x.Id == Model.Id));
            return $"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}";
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(Types.Get(typeInfo, "List"));
            return result;
        }

        private IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            var parameters = GetDecorators()
                .SelectMany(s => s.GetConstructorDependencies(this.Model))
                .Distinct()
                .ToArray();
            return parameters;
        }

        private string GetImplementation(IOperation operation)
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
