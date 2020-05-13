using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.Custom
{
    partial class CustomTemplateRegistrationTemplate : IntentRoslynProjectItemTemplateBase<TemplateRegistrationModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.Custom";

        public CustomTemplateRegistrationTemplate(IProject project, TemplateRegistrationModel model) : base(TemplateId, project, model)
        {
            if (!string.IsNullOrWhiteSpace(Model.GetModeler().GetDesignerSettings().NuGetPackageId()) &&
                !string.IsNullOrWhiteSpace(Model.GetModeler().GetDesignerSettings().NuGetPackageVersion()))
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

        private string GetTemplateNameForTemplateId()
        {
            return Model.Name.Replace("Registrations", "Template");
        }
    }
}
