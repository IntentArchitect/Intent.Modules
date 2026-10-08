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
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.HooksJson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class HooksJsonTemplate : IntentTemplateBase<HarnessFolderModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.HooksJson";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public HooksJsonTemplate(IOutputTarget outputTarget, HarnessFolderModel model = null) : base(TemplateId, outputTarget, model)
        {
        }

        /// <summary>
        /// The template for harnesses that follow the "hooks.json" convention: a trigger-keyed object
        /// of matcher/hooks entries, written at the root of the harness's own folder. Currently only
        /// Codex.
        /// </summary>
        /// <remarks>
        /// Four harnesses are deliberately absent, not overlooked. Claude Code's config lives in
        /// ".claude/settings.json" - see ClaudeSettings. OpenCode enforces through a TypeScript
        /// plugin - see OpenCodePlugin. Kiro and Cursor each have their own schema - see KiroHooks and
        /// CursorHooks. Keeping those as arms of a switch here meant one template owning four unrelated
        /// file shapes, and hid a real bug: Cursor's arm used a hook that cannot block.
        /// </remarks>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && HarnessFolder is ".codex";
        }

        /// <summary>
        /// The anchor folder this instance landed in, and therefore which harness it is generating
        /// for. Every template is offered every AI.Context anchor, so landing somewhere this one
        /// does not serve is the normal case rather than an error - it declines silently.
        /// </summary>
        private string HarnessFolder => OutputTarget.Name;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // Written INSIDE the anchor, never escaping with "../" - an escaped path resolves the
            // same from every anchor, which is what previously collapsed three instances onto one
            // file and stopped the Software Factory dead.
            return new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: "");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Merged, not owned: ".codex/hooks.json" is the project's one hooks file, and other tools
            // register their own entries in it. Owning it outright deleted theirs on every run. See
            // HarnessHookSets.MergeCodexHooks for what is registered, and why the write matcher and
            // the Windows command are what they are.
            return MergedHookFile.Transform(
                Path.Combine(OutputTarget.Location, "hooks.json"), TemplateId, "Codex", HarnessHookSets.MergeCodexHooks);
        }
    }
}
