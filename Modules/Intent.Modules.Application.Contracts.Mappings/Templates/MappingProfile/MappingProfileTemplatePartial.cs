using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    partial class MappingProfileTemplate : Intent.SoftwareFactory.Templates.IntentRoslynProjectItemTemplateBase<IList<DTOModel>>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Application.Contracts.Mapping.Profile";

        public MappingProfileTemplate(IProject project, IList<DTOModel> model)
            : base(Identifier, project, model)
        {
            ContractTemplateDependancyId = "Intent.Application.Contracts.DTO";
            DomainTemplateDependancyId = "Intent.RichDomain.EntityStateInterface";
        }

        public string ContractTemplateDependancyId { get; set; }
        public string DomainTemplateDependancyId { get; set; }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies().Concat(new INugetPackageInfo[] { NugetPackages.AutoMapper });
        }

        public string GetContractType(DTOModel model)
        {
            var templateDependancy = TemplateDependancy.OnModel<DTOModel>(ContractTemplateDependancyId, (to) => to.Id == model.Id);
            var templateOutput = this.Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {this.Id} Depends on : {ContractTemplateDependancyId} Model : {model.Id}");
            }
            return templateOutput.FullTypeName();
        }

        public string GetDomainType(DTOModel model)
        {
            var templateDependancy = TemplateDependancy.OnModel<Class>(DomainTemplateDependancyId, (to) => to.ClassId == model.MappedClassId);
            var templateOutput = this.Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {this.Id} Depends on : {DomainTemplateDependancyId} Model : {model.MappedClassId}");
            }
            return templateOutput.FullTypeName();
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "DtoProfile",
                fileExtension: "cs",
                defaultLocationInProject: "Mappings",
                className: "DtoProfile",
                @namespace: "${Project.ProjectName}"
                );
        }

        private string ToPascalCasePath(string path)
        {
            var piecies = path.Split('.');
            return piecies.Select(x => x.ToPascalCase()).Aggregate((x, y) => x + "." + y);
        }
    }
}
