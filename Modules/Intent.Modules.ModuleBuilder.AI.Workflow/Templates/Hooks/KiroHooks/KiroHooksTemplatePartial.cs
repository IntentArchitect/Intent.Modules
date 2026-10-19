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

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.KiroHooks
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class KiroHooksTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.KiroHooks";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public KiroHooksTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string HarnessFolder = ".kiro";

        /// <summary>
        /// Kiro has its own hook schema rather than the "hooks.json" convention the generic template
        /// serves, which is why it is a template of its own. Every template is offered every
        /// AI.Context anchor, so landing anywhere but ".kiro" is the normal case and declines
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
                fileName: "intent-agent-gate",
                fileExtension: "json",
                relativeLocation: "hooks"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Kiro's v3 contract: a flat "hooks" array of named entries, each with its own trigger,
            // rather than the trigger-keyed object the other harnesses use. PascalCase triggers and
            // "action.type": "command" are per its hooks reference.
            //
            // WHO READS THIS FILE. Per Kiro's docs, ".kiro/hooks/*.json" is the format of the Kiro IDE
            // (1.0+) and Kiro CLI v3. The CLI's default engine is still v2, which only reads hooks
            // embedded in an agent config and ignores this file - observed on CLI 2.22. Under
            // "kiro-cli --v3" it is read in interactive sessions only: headless "--no-interactive" runs
            // never load workspace hooks (Kiro issue #11598) - also observed.
            //
            // THE MATCHER IS STILL UNVERIFIED. v2 named the write tool "fs_write"; Kiro's tool
            // reference names it "write" (aliases "fs_write", "fsWrite"), and v3 splits edits into
            // further tools such as "str_replace" and "delete_file". The docs do not say whether a v3
            // matcher sees aliases, and a wrong matcher fails silently and OPEN, so every known name
            // is listed. Not ".*": guard-write would then also see read tools, which carry a path too,
            // and deny an agent reading metadata.
            //
            // The guards use the portable form because Kiro runs hooks in PowerShell on Windows, which
            // turned the gate's exit 2 into 1 and let a denied write through until
            // "; exit $LASTEXITCODE" restored it - observed. See GateCommands.GuardPortable.
            return $$"""
                {
                  "version": "v1",
                  "hooks": [
                    {
                      "name": "intent-agent-gate-warm",
                      "trigger": "SessionStart",
                      "action": { "type": "command", "command": "{{GateCommands.Warm(HarnessFolder)}}" }
                    },
                    {
                      "name": "intent-agent-gate-guard-write",
                      "trigger": "PreToolUse",
                      "matcher": "write|fs_write|fsWrite|str_replace|fs_append|delete_file",
                      "action": { "type": "command", "command": "{{GateCommands.GuardPortable(HarnessFolder, "guard-write")}}" }
                    },
                    {
                      "name": "intent-agent-gate-guard-version",
                      "trigger": "PreToolUse",
                      "matcher": ".*run_designer_script.*",
                      "action": { "type": "command", "command": "{{GateCommands.GuardPortable(HarnessFolder, "guard-version")}}" }
                    },
                    {
                      "name": "intent-agent-gate-close-out",
                      "trigger": "Stop",
                      "action": { "type": "command", "command": "{{GateCommands.CloseOut(HarnessFolder)}}" }
                    }
                  ]
                }
                """;
        }
    }
}