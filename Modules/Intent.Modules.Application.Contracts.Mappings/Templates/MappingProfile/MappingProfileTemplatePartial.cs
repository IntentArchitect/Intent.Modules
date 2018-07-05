using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    partial class MappingProfileTemplate : Intent.SoftwareFactory.Templates.IntentRoslynProjectItemTemplateBase<IList<IDTOModel>>, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.Application.Contracts.Mapping.Profile";
        public const string ContractTemplateDependancyId = "ContractTemplateDependancyId";
        public const string DomainTemplateDependancyId = "DomainTemplateDependancyId";

        public MappingProfileTemplate(IProject project, IList<IDTOModel> model)
            : base(Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies().Concat(new INugetPackageInfo[] { NugetPackages.AutoMapper });
        }

        public string GetContractType(IDTOModel model)
        {
            var templateDependancy = TemplateDependancy.OnModel<IDTOModel>(GetMetaData().CustomMetaData[ContractTemplateDependancyId], (to) => to.Id == model.Id);
            var templateOutput = this.Project.Application.FindTemplateInstance<IHasClassDetails>(templateDependancy);
            if (templateOutput == null)
            {
                Logging.Log.Failure($"Unable to resolve template dependancy. Template : {this.Id} Depends on : {ContractTemplateDependancyId} Model : {model.Id}");
            }
            return templateOutput.FullTypeName();
        }

        public string GetDomainType(IDTOModel model)
        {
            var templateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData[DomainTemplateDependancyId], (to) => to.Id == model.MappedClassId);
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

        public void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"using {Namespace};" },
                { InitializationRequiredEvent.CallKey, $"InitializeMapper();" },
                { InitializationRequiredEvent.MethodKey, $@"
        void InitializeMapper()
        {{
           AutoMapper.Mapper.Initialize(x => x.AddProfile(new {ClassName}()));
        }}" }
            });
        }

        private string ToPascalCasePath(string path)
        {
            var piecies = path.Split('.');
            return piecies.Select(x => x.ToPascalCase()).Aggregate((x, y) => x + "." + y);
        }
    }
}
