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

namespace ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileT4
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TypeScriptSingleFileT4Template : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.TypeScript.TypeScriptSingleFileT4";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TypeScriptSingleFileT4Template(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                className: $"TypeScriptSingleFileT4",
                fileName: $"type-script-single-file-t4"
            );
        }
    }
}