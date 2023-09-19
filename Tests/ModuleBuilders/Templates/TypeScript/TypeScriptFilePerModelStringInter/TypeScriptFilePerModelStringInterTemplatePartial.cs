using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.TypeScript.TypeScriptFilePerModelStringInter
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TypeScriptFilePerModelStringInterTemplate : TypeScriptTemplateBase<Intent.Modelers.Domain.Api.ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.TypeScript.TypeScriptFilePerModelStringInter";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TypeScriptFilePerModelStringInterTemplate(IOutputTarget outputTarget, Intent.Modelers.Domain.Api.ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                className: $"{Model.Name}",
                fileName: $"{Model.Name.ToKebabCase()}"
            );
        }
    }
}