using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Dart.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Dart.Templates.DartFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Dart.DartFilePerModelCustom
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DartFilePerModelCustomTemplate : DartTemplateBase<Intent.Modelers.Domain.Api.ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Dart.DartFilePerModelCustom";

        [IntentManaged(Mode.Fully)]
        public DartFilePerModelCustomTemplate(IOutputTarget outputTarget, Intent.Modelers.Domain.Api.ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new DartFileConfig(
                className: $"{Model.Name}",
                fileName: $"{Model.Name.ToKebabCase()}"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            throw new NotImplementedException("Implement custom template here");
        }

    }
}