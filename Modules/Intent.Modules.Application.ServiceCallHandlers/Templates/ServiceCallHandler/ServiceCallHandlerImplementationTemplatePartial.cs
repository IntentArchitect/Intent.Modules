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
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    partial class ServiceCallHandlerImplementationTemplate : IntentRoslynProjectItemTemplateBase<IOperation>, ITemplate, IHasTemplateDependencies, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.Application.ServiceCallHandlers.Handler";

        public ServiceCallHandlerImplementationTemplate(IProject project, IServiceModel service, IOperation model)
            : base(Identifier, project, model)
        {
            Service = service;
        }

        public IServiceModel Service { get; set; }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            var typeReferences = Model.Parameters.Select(x => x.Type).ToList();
            if (Model.ReturnType != null)
            {
                typeReferences.Add(Model.ReturnType.Type);
            }

            return typeReferences.Select(x => TemplateDependency.OnModel<IMetadataModel>(DTOTemplate.IDENTIFIER, m => m.Id == x.Element.Id)).ToArray();
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            // GCB - This is a hack to get the Intent.Framework.Domain package installed in the Application layer if repositories are detected.
            // Note: the project reference dependency on Repositories.Api is not needed in the .imodspec since all we are using is the constant value of RepositoryInterfaceTemplate.Identifier
            if (Project.FindTemplateInstances<IHasClassDetails>(TemplateDependency.OnTemplate(RepositoryInterfaceTemplate.Identifier)).Any())
            {
                return new[]
                    {
                        NugetPackages.IntentFrameworkDomain,
                    }
                    .Union(base.GetNugetDependencies())
                    .ToArray();
            }
            return base.GetNugetDependencies();
        }

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

        private string GetOperationDefinitionParameters(IOperation o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.Type)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(IOperation o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.ReturnType.Type)}>" : GetTypeName(o.ReturnType.Type);
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            //var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            //if (typeInfo.IsCollection)
            //{
            //    result = "List<" + result + ">";
            //}
            return NormalizeNamespace(Types.Get(typeInfo, "List<{0}>"));
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
