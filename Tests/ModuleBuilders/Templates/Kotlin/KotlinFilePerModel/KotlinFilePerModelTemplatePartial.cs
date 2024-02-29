using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Kotlin;
using Intent.Modules.Common.Kotlin.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Kotlin.KotlinFilePerModel
{
    [IntentManaged(Mode.Fully)]
    partial class KotlinFilePerModelTemplate : KotlinTemplateBase<Intent.Modelers.Domain.Api.ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Kotlin.KotlinFilePerModel";

        [IntentManaged(Mode.Fully)]
        public KotlinFilePerModelTemplate(IOutputTarget outputTarget, Intent.Modelers.Domain.Api.ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new KotlinFileConfig(
                className: $"{Model.Name}",
                package: this.GetPackageName(),
                relativeLocation: this.GetFolderPath()
            );
        }

    }
}