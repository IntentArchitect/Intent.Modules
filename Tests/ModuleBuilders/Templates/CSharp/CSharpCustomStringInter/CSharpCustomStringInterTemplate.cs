using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace ModuleBuilders.Templates.CSharp.CSharpCustomStringInter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CSharpCustomStringInterTemplate    {
        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            return $@"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace {Namespace}
{{
    public class {ClassName}
    {{{string.Join(@"
", GetMembers())}
    }}
}}";
        }

        [IntentManaged(Mode.Ignore)]
        private IEnumerable<string> GetMembers()
        {
            var members = new List<string>();

            // example: adding a constructor
            members.Add($@"
        public {ClassName}()
        {{
        }}");
            return members;
        }
    }
}