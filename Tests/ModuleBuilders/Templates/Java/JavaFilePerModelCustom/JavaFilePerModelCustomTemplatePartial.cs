using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Java;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Java.Templates.JavaFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Java.JavaFilePerModelCustom
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class JavaFilePerModelCustomTemplate : JavaTemplateBase<Intent.Modelers.Domain.Api.ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Java.JavaFilePerModelCustom";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public JavaFilePerModelCustomTemplate(IOutputTarget outputTarget, Intent.Modelers.Domain.Api.ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new JavaFileConfig(
                className: $"{Model.Name}",
                package: this.GetPackage(),
                relativeLocation: this.GetFolderPath()
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            throw new NotImplementedException("Implement custom template here");
        }
    }
}