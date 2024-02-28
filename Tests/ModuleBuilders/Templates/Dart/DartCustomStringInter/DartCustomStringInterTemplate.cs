using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly:IntentTemplate("Intent.ModuleBuilder.Dart.Templates.DartFileStringInterpolationTemplate",Version= "1.0")]

namespace ModuleBuilders.Templates.Dart.DartCustomStringInter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DartCustomStringInterTemplate
    {
        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return $@"
class {ClassName} {{{string.Join(Environment.NewLine, GetMembers())}
}}
";
        }

        private IEnumerable<string> GetMembers()
        {
            var members = new List<string>();

            // example: adding a constructor
            members.Add($@"
    {ClassName}() {{
    }}");

            return members;
        }
    }
}