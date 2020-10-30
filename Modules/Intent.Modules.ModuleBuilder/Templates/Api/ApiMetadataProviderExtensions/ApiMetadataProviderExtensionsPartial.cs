using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiMetadataProviderExtensions : CSharpTemplateBase<IList<ElementSettingsModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions";

        public ApiMetadataProviderExtensions(IOutputTarget project, IList<ElementSettingsModel> model) : base(TemplateId, project, model)
        {
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: "ApiMetadataProviderExtensions",
                @namespace: Model.First().ParentModule.ApiNamespace);
        }

        private string GetClassName(ElementSettingsModel elementSettings)
        {
            return GetTemplateClassName(ApiElementModel.ApiElementModel.TemplateId, elementSettings);
        }
    }
}