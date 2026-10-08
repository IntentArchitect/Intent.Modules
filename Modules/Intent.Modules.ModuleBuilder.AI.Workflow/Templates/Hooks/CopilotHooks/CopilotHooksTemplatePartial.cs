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

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.CopilotHooks
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CopilotHooksTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.CopilotHooks";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CopilotHooksTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string HarnessFolder = ".github";

        /// <summary>
        /// GitHub Copilot CLI reads repository hooks from ".github/hooks/NAME.json", so ".github" is
        /// this harness's anchor folder. Every template is offered every AI.Context anchor, so landing
        /// anywhere else is the normal case and declines silently.
        /// </summary>
        /// <remarks>
        /// ".github" differs from every other anchor this module writes into: it is NOT a
        /// Copilot-owned directory. It already exists in most repositories for workflows, issue
        /// templates and CODEOWNERS. So this template owns ".github/hooks/" and nothing above it -
        /// it adds files beside whatever is already there and never treats ".github" as its own.
        /// </remarks>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == HarnessFolder;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // Named rather than "hooks", because Copilot treats every *.json in this folder as a
            // hook file - an unrelated one may well be sitting beside it.
            return new TemplateFileConfig(
                fileName: "intent-agent-gate",
                fileExtension: "json",
                relativeLocation: "hooks"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Copilot's schema differs from every other harness in three ways, all per its own hooks
            // reference:
            //
            // 1. The command is carried in "bash" and/or "powershell" fields rather than a single
            //    "command", and Copilot picks the field for the platform. On Windows it runs the
            //    "powershell" one in PowerShell 7 - observed by driving Copilot CLI 1.0.91 for real.
            // 2. There is no "matcher". preToolUse fires for EVERY tool, so the gate self-filters:
            //    guard-write allows silently when it cannot find a file path in the payload, which is
            //    the overwhelming common case and costs nothing.
            // 3. The end-of-turn event is "agentStop", not "stop" or "Stop".
            //
            // Copilot denies on exit 2 and ALSO on any other non-zero exit ("hook errored"), so it
            // fails closed on its own. The guards are still wrapped: the wrapper turns a gate that
            // could not run into exit 2, which carries dotnet's own error text to the agent rather
            // than Copilot's bare "hook errored". close-out is never wrapped - see GateCommands.
            //
            // Copilot also runs the hooks in ".claude/settings.json" when that file is present, so
            // in a repository with both, Claude Code's commands run here too - which is why those
            // must also work under PowerShell (see GateCommands.GuardPosix).
            return $$"""
                {
                  "version": 1,
                  "hooks": {
                    "sessionStart": [
                      {
                        "type": "command",
                        "bash": "{{GateCommands.Warm(HarnessFolder)}}",
                        "powershell": "{{GateCommands.Warm(HarnessFolder)}}"
                      }
                    ],
                    "preToolUse": [
                      {
                        "type": "command",
                        "bash": "{{GateCommands.GuardPosix(HarnessFolder, "guard-write")}}",
                        "powershell": "{{GateCommands.GuardPowerShell(HarnessFolder, "guard-write")}}"
                      }
                    ],
                    "agentStop": [
                      {
                        "type": "command",
                        "bash": "{{GateCommands.CloseOut(HarnessFolder)}}",
                        "powershell": "{{GateCommands.CloseOut(HarnessFolder)}}"
                      }
                    ]
                  }
                }
                """;
        }
    }
}