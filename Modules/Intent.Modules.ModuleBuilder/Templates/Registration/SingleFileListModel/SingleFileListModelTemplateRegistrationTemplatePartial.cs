using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    partial class SingleFileListModelTemplateRegistrationTemplate : CSharpTemplateBase<TemplateRegistrationModel>, IDeclareUsings
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel";

        public SingleFileListModelTemplateRegistrationTemplate(IOutputTarget outputTarget, TemplateRegistrationModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);

            if (!string.IsNullOrWhiteSpace(Model.GetModelType()?.ParentModule.NuGetPackageId) &&
                outputTarget.GetProject().Name != Model.GetModelType().ParentModule.NuGetPackageId)
            {
                AddNugetDependency(new NugetPackageInfo(Model.GetModelType()?.ParentModule.NuGetPackageId, Model.GetModelType()?.ParentModule.NuGetPackageVersion));
            }
            if (!string.IsNullOrWhiteSpace(Model.GetDesigner()?.ParentModule.NuGetPackageId) &&
                outputTarget.GetProject().Name != Model.GetDesigner().ParentModule.NuGetPackageId)
            {
                AddNugetDependency(new NugetPackageInfo(Model.GetDesigner()?.ParentModule.NuGetPackageId, Model.GetDesigner()?.ParentModule.NuGetPackageVersion));
            }
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        public IList<string> OutputFolders => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name.ToCSharpIdentifier().RemoveSuffix("Template") }).ToList();

        public string FolderPath => string.Join("/", OutputFolders);

        public string FolderNamespace => string.Join(".", OutputFolders);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}Registration",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                relativeLocation: $"{FolderPath}");
        }

        private string GetTemplateNameForTemplateId()
        {
            return TemplateName;
        }

        public string GetModelsMethod()
        {
            var modelName = GetModelType();
            return $"_metadataManager.{Model.GetDesigner().Name.ToCSharpIdentifier()}(application).Get{modelName.ToPluralName()}()";
        }

        public string GetModelType()
        {
            return NormalizeNamespace(Model.GetModelType()?.ClassName ?? Model.GetTemplateSettings().ModelName());
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (Model.GetModelType() != null)
            {
                yield return Model.GetModelType().ParentModule.ApiNamespace;
            }

            if (Model.GetDesigner() != null)
            {
                yield return Model.GetDesigner().ParentModule.ApiNamespace;
            }
        }
    }
}