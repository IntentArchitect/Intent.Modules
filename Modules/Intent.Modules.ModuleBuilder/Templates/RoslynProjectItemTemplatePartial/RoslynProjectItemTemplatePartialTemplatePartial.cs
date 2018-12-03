using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial
{
    partial class RoslynProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<IAttribute>
    {
        public const string TemplateId = "Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial";

        public RoslynProjectItemTemplatePartialTemplate(string templateId, IProject project, IAttribute model) : base(templateId, project, model)
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
                fileName: $"${{Model.Name}}Partial",
                fileExtension: "cs",
                defaultLocationInProject: "Templates\\${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.Templates.${Model.Name}"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
                {
                    NugetPackages.IntentArchitectPackager,
                    NugetPackages.IntentModulesCommon,
                    NugetPackages.IntentRoslynWeaverAttributes
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }

        private string GetTargetModel()
        {
            return "IClass";
        }

        private string GetTemplateId()
        {
            return $"{Project.Name}.{Model.Name}";
        }
    }
}
