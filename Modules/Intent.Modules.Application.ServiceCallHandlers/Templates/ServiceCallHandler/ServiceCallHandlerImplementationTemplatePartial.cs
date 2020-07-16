using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    partial class ServiceCallHandlerImplementationTemplate : IntentRoslynProjectItemTemplateBase<OperationModel>, ITemplate, IHasTemplateDependencies, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Application.ServiceCallHandlers.Handler";

        public ServiceCallHandlerImplementationTemplate(IProject project, ServiceModel service, OperationModel model)
            : base(Identifier, project, model)
        {
            SetDefaultTypeCollectionFormat("List<{0}>");
            AddTypeSource(CSharpTypeSource.InProject(Project, DTOTemplate.IDENTIFIER, "List<{0}>"));
            Service = service;
        }

        public ServiceModel Service { get; set; }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}SCH",
                fileExtension: "cs",
                defaultLocationInProject: $"ServiceCallHandlers/{Service.Name}",
                className: "${Model.Name}SCH",
                @namespace: "${Project.ProjectName}.ServiceCallHandlers.${Service.Name}"
                );
        }

        private string GetOperationDefinitionParameters(OperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.TypeReference)}>" : GetTypeName(o.TypeReference);
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { ContainerRegistrationEvent.InterfaceTypeKey, null},
                { ContainerRegistrationEvent.ConcreteTypeKey, $"{Namespace}.{ClassName}" },
                { ContainerRegistrationEvent.InterfaceTypeTemplateIdKey, null },
                { ContainerRegistrationEvent.ConcreteTypeTemplateIdKey, Identifier },
                { ContainerRegistrationEvent.LifetimeKey, ContainerRegistrationEvent.TransientLifetime }
            });
        }
    }
}
