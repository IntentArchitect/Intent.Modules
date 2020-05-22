using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    partial class SingleFileListModelTemplateRegistrationTemplate : IntentRoslynProjectItemTemplateBase<TemplateRegistrationModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel";

        public SingleFileListModelTemplateRegistrationTemplate(IProject project, TemplateRegistrationModel model) : base(TemplateId, project, model)
        {
            if (!string.IsNullOrWhiteSpace(Model.GetModeler()?.GetDesignerSettings().NuGetPackageId()) &&
                !string.IsNullOrWhiteSpace(Model.GetModeler()?.GetDesignerSettings().NuGetPackageVersion()))
            {
                AddNugetDependency(packageName: Model.GetModeler().GetDesignerSettings().NuGetPackageId(), packageVersion: Model.GetModeler().GetDesignerSettings().NuGetPackageVersion());
            }
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);
        public string FolderNamespace => string.Join(".", FolderBaseList);

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Registration",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}",
                className: "${Model.Name}Registration",
                @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.IntentModulesCommon
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
        
        private string GetTemplateNameForTemplateId()
        {
            return Model.Name.Replace("Registrations", "Template");
        }

        public string GetModelsMethod()
        {
            var modelName = Model.GetModelType().ClassName;
            return $"_metadataManager.Get{modelName.ToPluralName()}(application)";
        }

        public string GetModelType()
        {
            return Model.GetModelType()?.ClassName ?? Model.GetTemplateSettings().ModelName();
        }
    }
}