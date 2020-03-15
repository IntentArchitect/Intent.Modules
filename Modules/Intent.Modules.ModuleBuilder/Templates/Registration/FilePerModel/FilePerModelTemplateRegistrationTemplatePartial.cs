using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.FilePerModel
{
    partial class FilePerModelTemplateRegistrationTemplate : IntentRoslynProjectItemTemplateBase<IFileTemplate>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.FilePerModel";

        public FilePerModelTemplateRegistrationTemplate(IProject project, IFileTemplate model) : base(TemplateId, project, model)
        {
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

        private IModelerReference GetModeler()
        {
            return Model.GetFileTemplateSettings().Modeler() != null ? new ModelerReference(Model.GetFileTemplateSettings().Modeler()) : null;
        }

        private string GetModelType()
        {
            var modelType = Model.GetFileTemplateSettings().ModelType() != null ? new ModelerModelType(Model.GetFileTemplateSettings().ModelType()) : null;
            return modelType?.InterfaceName ?? "object";
        }

        public string GetModelsMethod()
        {
            var model = new ModelerModelType(Model.GetFileTemplateSettings().ModelType());
            return $"_metadataManager.Get{model.ClassName.ToPluralName()}(application)";
        }
    }
}