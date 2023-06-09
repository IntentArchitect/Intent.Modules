using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.ReleaseNotes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ReleaseNotesTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return @$"### Version 1.0.0
";
        }
    }
}