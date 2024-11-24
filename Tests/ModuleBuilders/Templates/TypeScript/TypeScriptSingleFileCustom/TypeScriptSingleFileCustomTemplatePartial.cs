using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileCustom
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class TypeScriptSingleFileCustomTemplate : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.TypeScript.TypeScriptSingleFileCustom";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TypeScriptSingleFileCustomTemplate(IOutputTarget outputTarget, object model = null)
             : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                className: $"TypeScriptSingleFileCustom",
                fileName: $"type-script-single-file-custom");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            throw new NotImplementedException("Implement custom template here");
        }
    }
}