using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    partial class SingleFileListModelTemplateRegistrationTemplate : CSharpTemplateBase<TemplateRegistrationModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel";

        public SingleFileListModelTemplateRegistrationTemplate(IProject project, TemplateRegistrationModel model) : base(TemplateId, project, model)
        {
            if (!string.IsNullOrWhiteSpace(Model.GetModelType()?.ParentModule.NuGetPackageId))
            {
                AddNugetDependency(new NugetPackageInfo(Model.GetModelType()?.ParentModule.NuGetPackageId, Model.GetModelType()?.ParentModule.NuGetPackageVersion));
            }
        }

        public IList<string> OutputFolder => Model.GetFolderPath().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"{Model.Name}Registration",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                relativeLocation: $"{FolderPath}");
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
            var modelName = GetModelType();
            return $"_metadataManager.{Model.GetDesigner().Name.ToCSharpIdentifier()}(application).Get{modelName.ToPluralName()}()";
        }

        public string GetModelType()
        {
            return Model.GetModelType()?.ClassName ?? Model.GetTemplateSettings().ModelName();
        }
    }
}