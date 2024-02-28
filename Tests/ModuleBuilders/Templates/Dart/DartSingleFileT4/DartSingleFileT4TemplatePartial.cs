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

namespace ModuleBuilders.Templates.Dart.DartSingleFileT4
{
    [IntentManaged(Mode.Fully)]
    partial class DartSingleFileT4Template : DartTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Dart.DartSingleFileT4";

        [IntentManaged(Mode.Fully)]
        public DartSingleFileT4Template(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new DartFileConfig(
                className: $"DartSingleFileT4",
                fileName: $"dart-single-file-t4"
            );
        }

    }
}