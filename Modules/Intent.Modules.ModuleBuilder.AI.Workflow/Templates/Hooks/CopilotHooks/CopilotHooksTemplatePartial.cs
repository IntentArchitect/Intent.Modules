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
            //    "command". Both are supplied, so the gate runs whichever shell Copilot picks - the
            //    bash form uses the usual fail-closed wrapper, and the PowerShell form spells the same
            //    thing with $LASTEXITCODE because "test $?" is not PowerShell.
            // 2. There is no "matcher". preToolUse fires for EVERY tool, so the gate self-filters:
            //    guard-write allows silently when it cannot find a file path in the payload, which is
            //    the overwhelming common case and costs nothing.
            // 3. The end-of-turn event is "agentStop", not "stop" or "Stop".
            //
            // UNVERIFIED: how Copilot signals a denial is not stated in its documentation, so exit 2
            // is assumed here for consistency with every other harness. If Copilot instead treats a
            // non-zero exit as advisory, this guard reports but does not block - which is exactly the
            // silent-and-open failure the matcher work elsewhere guards against, and is the first
            // thing to confirm when Copilot is actually driven.
            return $$"""
                {
                  "version": 1,
                  "hooks": {
                    "sessionStart": [
                      {
                        "type": "command",
                        "bash": "{{GateCommands.Warm(HarnessFolder)}}",
                        "powershell": "{{PowerShellWarm()}}"
                      }
                    ],
                    "preToolUse": [
                      {
                        "type": "command",
                        "bash": "{{GateCommands.Guard(HarnessFolder, "guard-write")}}",
                        "powershell": "{{PowerShellGuard("guard-write")}}"
                      }
                    ],
                    "agentStop": [
                      {
                        "type": "command",
                        "bash": "{{GateCommands.Guard(HarnessFolder, "close-out")}}",
                        "powershell": "{{PowerShellGuard("close-out")}}"
                      }
                    ]
                  }
                }
                """;
        }

        private static string PowerShellWarm() =>
            $"dotnet run {GateCommands.GatePath(HarnessFolder)} -- warm";

        /// <summary>
        /// The PowerShell spelling of the fail-closed wrapper: any non-zero exit becomes 2, the
        /// universal block signal, so a broken or missing gate cannot wave an action through.
        /// </summary>
        private static string PowerShellGuard(string command) =>
            $"dotnet run {GateCommands.GatePath(HarnessFolder)} --no-build -- {command} --harness {GateCommands.HarnessId(HarnessFolder)}; "
            + "if ($LASTEXITCODE -ne 0) { exit 2 }";
    }
}