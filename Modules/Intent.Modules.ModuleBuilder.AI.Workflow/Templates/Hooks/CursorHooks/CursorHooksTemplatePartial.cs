using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.CursorHooks
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CursorHooksTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.CursorHooks";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CursorHooksTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string HarnessFolder = ".cursor";

        /// <summary>
        /// Cursor has its own hook schema - camelCase event names, a "version" integer, and a
        /// per-entry "failClosed" flag - rather than the "hooks.json" convention the generic template
        /// serves, which is why it is a template of its own. Every template is offered every
        /// AI.Context anchor, so landing anywhere but ".cursor" is the normal case and declines
        /// silently.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == HarnessFolder;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // Written INSIDE the anchor. An escaped "../" path resolves identically from every
            // anchor, which is what previously collapsed several instances onto one file and stopped
            // the Software Factory dead.
            return new TemplateFileConfig(
                fileName: "hooks",
                fileExtension: "json",
                relativeLocation: ""
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Merged, not owned: Cursor allows exactly one project hooks file, so any other tool's
            // hooks live in this same file, and owning it outright deleted them on every run. See
            // HarnessHookSets.MergeCursorHooks for what is registered and why.
            return MergedHookFile.Transform(
                Path.Combine(OutputTarget.Location, "hooks.json"), TemplateId, "Cursor", HarnessHookSets.MergeCursorHooks);
        }
    }
}
