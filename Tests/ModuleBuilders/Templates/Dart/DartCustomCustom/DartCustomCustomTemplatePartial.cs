using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Dart.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Dart.Templates.DartFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Dart.DartCustomCustom
{
    [IntentManaged(Mode.Fully)]
    partial class DartCustomCustomTemplate : DartTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Dart.DartCustomCustom";

        [IntentManaged(Mode.Fully)]
        public DartCustomCustomTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new DartFileConfig(
                className: $"DartCustomCustom",
                fileName: $"dart-custom-custom"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            throw new NotImplementedException("Implement custom template here");
        }
    }
}