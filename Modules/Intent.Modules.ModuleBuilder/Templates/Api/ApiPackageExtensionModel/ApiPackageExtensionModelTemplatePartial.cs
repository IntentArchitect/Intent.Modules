using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiPackageExtensionModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiPackageExtensionModelTemplate : CSharpTemplateBase<PackageExtensionModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiPackageExtensionModelTemplate(IOutputTarget outputTarget, PackageExtensionModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: Model.ParentModule.ApiNamespace);
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            var module = new IntentModuleModel(GetBasePackageModel().InternalElement.Package);
            if (!CanRunTemplate() ||
                string.IsNullOrWhiteSpace(module.NuGetPackageId) ||
                string.IsNullOrWhiteSpace(module.NuGetPackageVersion) ||
                OutputTarget.GetProject().Name == module.NuGetPackageId)
            {
                return;
            }

            AddNugetDependency(module.NuGetPackageId, module.NuGetPackageVersion);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   Model.MenuOptions?.ElementCreations.Any() == true;
        }

        private PackageSettingsModel GetBasePackageModel()
        {
            return new PackageSettingsModel((IElement)Model.TypeReference.Element);
        }

        private static string FormatForCollection(string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        private static string GetCreationOptionName(ElementCreationOptionModel option)
        {
            if (option.GetOptionSettings().ApiModelName() != null)
            {
                return option.GetOptionSettings().ApiModelName();
            }
            var name = option.Name.Replace("Add ", "").Replace("New ", "").ToCSharpIdentifier();
            return option.GetOptionSettings().AllowMultiple() ? name.ToPluralName() : name;
        }
    }
}