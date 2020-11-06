using Intent.Engine;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.Templates;
using System.Collections.Generic;
using Intent.Modules.Angular.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Shared.IntentDecoratorsTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class IntentDecoratorsTemplate : TypeScriptTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Shared.IntentDecoratorsTemplate.IntentDecoratorsTemplate";

        public IntentDecoratorsTemplate(IOutputTarget project, object model) : base(TemplateId, project, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "intent.decorators",
                relativeLocation: "",
                className: "IntentIgnore, IntentMerge, IntentManage"
            );
        }

    }
}