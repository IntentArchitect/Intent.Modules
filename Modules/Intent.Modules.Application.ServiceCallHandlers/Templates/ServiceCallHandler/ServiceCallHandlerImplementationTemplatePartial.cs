using Intent.MetaModel.Common;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.DTO;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    partial class ServiceCallHandlerImplementationTemplate : IntentRoslynProjectItemTemplateBase<IOperationModel>, ITemplate, IHasTemplateDependencies, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.Application.ServiceCallHandlers.Handler";

        public ServiceCallHandlerImplementationTemplate(IProject project, IServiceModel serviceModel, IOperationModel model)
            : base(Identifier, project, model)
        {
            ServiceModel = serviceModel;
            Context.AddFakeProperty("Service", ServiceModel);
        }

        public IServiceModel ServiceModel { get; set; }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            var typeReferences = Model.Parameters.Select(x => x.TypeReference).ToList();
            if (Model.ReturnType != null)
            {
                typeReferences.Add(Model.ReturnType.TypeReference);
            }

            return typeReferences.Select(x => TemplateDependancy.OnModel<IDTOModel>(DTOTemplate.IDENTIFIER, m => m.Id == x.Id)).ToArray();
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
                {
                    NugetPackages.IntentFrameworkDomain,
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}SCH",
                fileExtension: "cs",
                defaultLocationInProject: $"ServiceCallHandlers\\{ServiceModel.Name}",
                className: "${Model.Name}SCH",
                @namespace: "${Project.ProjectName}.ServiceCallHandlers.${Service.Name}"
                );
        }

        private string GetOperationDefinitionParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(IOperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.ReturnType.TypeReference)}>" : GetTypeName(o.ReturnType.TypeReference);
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            if (typeInfo.IsCollection)
            {
                result = "List<" + result + ">";
            }
            return result;
        }

        public void BeforeTemplateExecution()
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
