using System.Collections.Generic;
using System.Linq;
using Intent.Packages.Application.ServiceCallHandlers.Legacy.ServiceCallHandler;
using Intent.Packages.Application.Contracts.Legacy.DTO;
using Intent.Packages.Application.Contracts.Legacy.ServiceContract;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using ServiceCallHandlerImplementationTemplate = Intent.Packages.Application.ServiceCallHandlers.Templates.ServiceCallHandler.ServiceCallHandlerImplementationTemplate;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Application.ServiceCallHandlers.Legacy.ServiceImplementation
{
    partial class ServiceImplementationTemplate : IntentRoslynProjectItemTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Application.ServiceCallHandlers.ServiceImplementation.Legacy";
        public ServiceImplementationTemplate(ServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DTOTemplate.Identifier),
                TemplateDependancy.OnTemplate(ServiceContractTemplate.Identifier),
            }
            .Union(Model.Operations.Select(x => TemplateDependancy.OnModel(ServiceCallHandlerImplementationTemplate.Identifier, x)).ToArray());
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.CommonServiceLocator,
                NugetPackages.Unity,
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
                fileName: "${Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Generated\\ServiceImplementation",
                className: "${Name}",
                @namespace: "${Project.ProjectName}"

                );
        }
    }
}
