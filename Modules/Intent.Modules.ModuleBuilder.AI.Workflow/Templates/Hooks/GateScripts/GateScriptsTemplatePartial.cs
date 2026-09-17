using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.GateScripts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class GateScriptsTemplate : IntentTemplateBase<GateSourceFileModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.GateScripts";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public GateScriptsTemplate(IOutputTarget outputTarget, GateSourceFileModel model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(fileName: Model.Name, fileExtension: Model.Extension, relativeLocation: "../.agents/hooks");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return Model.Content;
        }
    }
}