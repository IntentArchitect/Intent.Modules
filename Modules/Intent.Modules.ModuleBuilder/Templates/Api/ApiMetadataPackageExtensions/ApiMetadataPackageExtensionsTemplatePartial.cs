using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiMetadataPackageExtensionsTemplate : CSharpTemplateBase<IList<PackageSettingsModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiMetadataPackageExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiMetadataPackageExtensionsTemplate(IOutputTarget outputTarget, IList<PackageSettingsModel> model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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