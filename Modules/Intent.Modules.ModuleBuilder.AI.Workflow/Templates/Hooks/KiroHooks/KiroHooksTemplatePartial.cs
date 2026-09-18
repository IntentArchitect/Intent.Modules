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
            // UNVERIFIED BY EXECUTION. The "fs_write" matcher is Kiro's documented tool name, but no
            // hook has ever run to confirm it: CLI 2.22.0 / KAS 0.66.0 does not execute
            // ".kiro/hooks/*.json" at all. A wrong matcher fails silently and OPEN - the config looks
            // correct and simply never matches - so this is the first thing to re-check once a Kiro
            // build actually runs hooks.
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
                      "matcher": "fs_write",
                      "action": { "type": "command", "command": "{{GateCommands.Guard(HarnessFolder, "guard-write")}}" }
                    },
                    {
                      "name": "intent-agent-gate-guard-version",
                      "trigger": "PreToolUse",
                      "matcher": ".*run_designer_script.*",
                      "action": { "type": "command", "command": "{{GateCommands.Guard(HarnessFolder, "guard-version")}}" }
                    },
                    {
                      "name": "intent-agent-gate-close-out",
                      "trigger": "Stop",
                      "action": { "type": "command", "command": "{{GateCommands.Guard(HarnessFolder, "close-out")}}" }
                    }
                  ]
                }
                """;
        }
    }
}