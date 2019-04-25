using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial
{
    partial class ProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        public ProjectItemTemplatePartialTemplate(string templateId, IProject project, IClass model) : base(templateId, project, model)
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
                defaultLocationInProject: "Templates/${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.Templates.${Model.Name}"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
                {
                    NugetPackages.IntentModulesCommon,
                    NugetPackages.IntentRoslynWeaverAttributes
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }

        private string GetTemplateId()
        {
            return $"{Project.ApplicationName()}.{Model.Name}";
        }

        private string GetModelType()
        {
            var type = Model.GetTargetModel();
            if (Model.GetRegistrationType() == RegistrationType.SingleFileListModel)
            {
                type = $"IList<{type}>";
            }

            return type;
        }
    }
}
