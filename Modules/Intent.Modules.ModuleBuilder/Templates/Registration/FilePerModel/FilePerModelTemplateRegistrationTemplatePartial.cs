using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.FilePerModel
{
    partial class FilePerModelTemplateRegistrationTemplate : IntentRoslynProjectItemTemplateBase<IElement>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.FilePerModel";

        public FilePerModelTemplateRegistrationTemplate(IProject project, IElement model) : base(TemplateId, project, model)
        {
        }

        public string FolderPath => string.Join("\\", new[] { "Templates" }.Concat(Model.GetFolderPath().Select(x => x.Name).ToList()));
        public string FolderNamespace => string.Join(".", new[] { "Templates" }.Concat(Model.GetFolderPath().Select(x => x.Name).ToList()));

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
    }
}