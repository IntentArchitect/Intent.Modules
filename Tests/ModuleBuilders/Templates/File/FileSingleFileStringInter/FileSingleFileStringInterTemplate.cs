using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace ModuleBuilders.Templates.File.FileSingleFileStringInter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FileSingleFileStringInterTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return @$"// Place your file template logic here
";
        }
    }
}