using System.Collections.Generic;
using Intent.Packages.Application.Contracts.Legacy.DTO;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Application.ServiceCallHandlers.Legacy.ServiceCallHandler
{
    partial class ServiceCallHandlerImplementationTemplate : IntentRoslynProjectItemTemplateBase<ServiceOperationModel>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Application.ServiceCallHandlers.Handler.Legacy";

        public ServiceCallHandlerImplementationTemplate(ServiceModel serviceModel, ServiceOperationModel model, IProject project)
            : base(Identifier, project, model)
        {
            ServiceModel = serviceModel;
            Context.AddFakeProperty("Service", ServiceModel);
        }

        /*
        public override string DependencyUsings
        {
            get
            {
                var x = this.ResolveAllUsings(Project);
                return x;
            }
        }*/

        public ServiceModel ServiceModel { get; set; }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DTOTemplate.Identifier),
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Name}SCH",
                fileExtension: "cs",
                defaultLocationInProject: $"ServiceCallHandlers\\{ServiceModel.Name}",
                className: "${Name}SCH",
                @namespace: "${Project.ProjectName}"// + ServiceModel.Name // TODO: One day...
                );
        }
    }
}
