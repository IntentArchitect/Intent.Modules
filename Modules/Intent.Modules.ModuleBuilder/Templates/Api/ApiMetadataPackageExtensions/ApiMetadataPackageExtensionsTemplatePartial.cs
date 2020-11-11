using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiMetadataPackageExtensionsTemplate : CSharpTemplateBase<IList<PackageSettingsModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions";

        public ApiMetadataPackageExtensionsTemplate(IOutputTarget project, IList<PackageSettingsModel> model) : base(TemplateId, project, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "ApiMetadataPackageExtensions",
                @namespace: Model.First().ParentModule.ApiNamespace);
        }

        private string GetClassName(PackageSettingsModel packageSettings)
        {
            return GetTypeName(ApiPackageModel.ApiPackageModelTemplate.TemplateId, packageSettings);
        }

    }
}