using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.PerModel
{
    partial class PerModelTemplateRegistrationTemplate : IntentRoslynProjectItemTemplateBase<IAttribute>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.PerModel";

        public PerModelTemplateRegistrationTemplate(IProject project, IAttribute model) : base(PerModelTemplateRegistrationTemplate.TemplateId, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Registration",
                fileExtension: "cs",
                defaultLocationInProject: "Templates\\${Model.Name}",
                className: "${Model.Name}Registration",
                @namespace: "${Project.Name}.Templates.${Model.Name}"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.IntentArchitectPackager,
                NugetPackages.IntentModulesCommon
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        private string GetModelType()
        {
            return "IClass";
        }

        private string GetTemplateNameForTemplateId()
        {
            return Model.Name.Replace("Registrations", "Template");
        }
    }
}