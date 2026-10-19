using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace ModuleBuilders.Templates.CSharp.CSharpFilePerModelStringInter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CSharpFilePerModelStringInterTemplate    {
        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            return $$"""
                     using System;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}}
                     {
                         public class {{ClassName}}
                         {
                         }
                     }
                     """;
        }
    }
}