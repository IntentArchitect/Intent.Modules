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
            // Nested inside whichever AI.Context anchor this instance landed in - ".agents/hooks/gate",
            // ".claude/hooks/gate", and so on - so each harness carries its own self-contained copy.
            // Deliberately NOT escaped with "../": an escaped path resolves identically from every
            // anchor, so more than one anchor collapses to a single output and the Software Factory
            // refuses to run (DuplicateOutputPathException). Staying inside the anchor makes that
            // impossible by construction.
            return new TemplateFileConfig(fileName: Model.Name, fileExtension: Model.Extension, relativeLocation: "hooks/gate");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return Model.Content;
        }
    }
}