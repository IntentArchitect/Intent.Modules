using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Kotlin;
using Intent.Modules.Common.Kotlin.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Kotlin.KotlinSingleFile
{
    [IntentManaged(Mode.Fully)]
    partial class KotlinSingleFileTemplate : KotlinTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Kotlin.KotlinSingleFile";

        [IntentManaged(Mode.Fully)]
        public KotlinSingleFileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new KotlinFileConfig(
                className: $"KotlinSingleFile",
                package: this.GetPackageName(),
                relativeLocation: this.GetFolderPath()
            );
        }

    }
}