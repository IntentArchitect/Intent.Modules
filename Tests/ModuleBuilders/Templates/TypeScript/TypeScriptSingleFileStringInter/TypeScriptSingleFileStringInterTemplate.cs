using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplateStringInterpolation", Version = "1.0")]

namespace ModuleBuilders.Templates.TypeScript.TypeScriptSingleFileStringInter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TypeScriptSingleFileStringInterTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $@"export class {ClassName} {{{string.Join(Environment.NewLine, GetMembers())}
}}
";
        }

        private IEnumerable<string> GetMembers()
        {
            var members = new List<string>();

            // example: adding a constructor
            members.Add($@"
    constructor() {{
    }}");
            return members;
        }
    }
}