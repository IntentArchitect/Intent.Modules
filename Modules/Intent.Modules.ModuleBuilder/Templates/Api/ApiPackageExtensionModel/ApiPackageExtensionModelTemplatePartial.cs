using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiPackageExtensionModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiPackageExtensionModelTemplate : CSharpTemplateBase<PackageExtensionModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiPackageExtensionModel";

        public ApiPackageExtensionModelTemplate(IOutputTarget outputTarget, PackageExtensionModel model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private PackageSettingsModel GetBasePackageModel()
        {
            return new PackageSettingsModel((IElement)Model.TypeReference.Element);
        }

        private string FormatForCollection(string name, bool asCollection)
        {
            return asCollection ? $"IList<{name}>" : name;
        }

        private string GetCreationOptionName(ElementCreationOptionModel option)
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