using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.MetaModel.DTO;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    partial class MappingProfileTemplate : IntentRoslynProjectItemTemplateBase<IList<IDTOModel>>, IBeforeTemplateExecutionHook
    {
        public const string IDENTIFIER = "Intent.Application.Contracts.Mapping.Profile";
        public const string CONTRACT_TEMPLATE_DEPENDANCY_ID = "ContractTemplateDependancyId";
        public const string DOMAIN_TEMPLATE_DEPENDANCY_ID = "DomainTemplateDependancyId";

        public MappingProfileTemplate(IProject project, IList<IDTOModel> model)
            : base(IDENTIFIER, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies().Concat(new[] { NugetPackages.AutoMapper });
        }

        public string GetContractType(IDTOModel model)
        {
            var templateDependancy = TemplateDependancy.OnModel<IDTOModel>(GetMetaData().CustomMetaData[CONTRACT_TEMPLATE_DEPENDANCY_ID], (to) => to.Id == model.Id);
            var templateOutput = Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {Id} Depends on : {CONTRACT_TEMPLATE_DEPENDANCY_ID} Model : {model.Id}");
            }
            return templateOutput.FullTypeName();
        }

        public string GetDomainType(IDTOModel model)
        {
            var templateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData[DOMAIN_TEMPLATE_DEPENDANCY_ID], (to) => to.Id == model.MappedClassId);
            var templateOutput = Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {Id} Depends on : {DOMAIN_TEMPLATE_DEPENDANCY_ID} Model : {model.MappedClassId}");
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

        public void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"{Namespace};" },
                { InitializationRequiredEvent.CallKey, "InitializeMapper();" },
                { InitializationRequiredEvent.MethodKey, $@"
        private void InitializeMapper()
        {{
           AutoMapper.Mapper.Initialize(x => x.AddProfile(new {ClassName}()));
        }}" },
                { InitializationRequiredEvent.TemplateDependencyIdKey, IDENTIFIER }
            });
        }

        private string ToPascalCasePath(string path)
        {
            var piecies = path.Split('.');
            return piecies.Select(x => x.ToPascalCase()).Aggregate((x, y) => x + "." + y);
        }
    }
}
