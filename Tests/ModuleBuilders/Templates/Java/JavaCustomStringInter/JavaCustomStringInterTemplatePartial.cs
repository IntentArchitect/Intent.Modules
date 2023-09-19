using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Java;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Java.Templates.JavaFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Java.JavaCustomStringInter
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class JavaCustomStringInterTemplate : JavaTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Java.JavaCustomStringInter";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public JavaCustomStringInterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new JavaFileConfig(
                className: $"JavaCustomStringInter",
                package: this.GetPackage(),
                relativeLocation: this.GetFolderPath()
            );
        }
    }
}