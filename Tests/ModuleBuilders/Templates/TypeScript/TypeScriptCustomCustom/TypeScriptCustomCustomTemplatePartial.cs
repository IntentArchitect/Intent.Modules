using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.TypeScript.TypeScriptCustomCustom
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class TypeScriptCustomCustomTemplate : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.TypeScript.TypeScriptCustomCustom";

        [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
        public TypeScriptCustomCustomTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                className: $"TypeScriptCustomCustom",
                fileName: $"type-script-custom-custom"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            throw new NotImplementedException("Implement custom template here");
        }
    }
}