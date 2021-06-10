using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.Custom
{
    partial class CustomTemplateRegistrationTemplate : CSharpTemplateBase<TemplateRegistrationModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.Custom";

        public CustomTemplateRegistrationTemplate(IOutputTarget project, TemplateRegistrationModel model) : base(TemplateId, project, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);

            if (!string.IsNullOrWhiteSpace(Model.GetModelType()?.ParentModule.NuGetPackageId))
            {
                AddNugetDependency(new NugetPackageInfo(Model.GetModelType()?.ParentModule.NuGetPackageId, Model.GetModelType()?.ParentModule.NuGetPackageVersion));
            }
        }

        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Template")}TemplateRegistration",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                relativeLocation: $"{FolderPath}");
        }

        private string GetTemplateNameForTemplateId()
        {
            return $"{Model.Name.Replace("Registrations", "Template").RemoveSuffix("Template")}Template";
        }
    }
}
