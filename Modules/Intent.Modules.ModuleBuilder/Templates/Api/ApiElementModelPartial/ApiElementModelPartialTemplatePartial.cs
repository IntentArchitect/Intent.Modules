using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelPartial
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiElementModelPartialTemplate : CSharpTemplateBase<ElementSettingsModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiElementModelPartial";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiElementModelPartialTemplate(IOutputTarget outputTarget, ElementSettingsModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.ApiModelName}",
                @namespace: Model.ParentModule.ApiNamespace,
                fileName: $"{Model.ApiModelName}.partial");
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetModuleBuilderSettings().CreatePartialAPIModels();
        }
    }
}