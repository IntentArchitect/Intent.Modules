using System;
using System.Collections.Generic;
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
        /// Claude Code and OpenCode are deliberately absent from the switches below, not overlooked.
        /// Claude's config lives in ".claude/settings.json", a file shared with the developer's own
        /// permissions and environment, so it has to be merged rather than owned outright - see
        /// ClaudeSettings. OpenCode enforces through a TypeScript plugin - see OpenCodePlugin.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && HarnessFolder is ".codex" or ".kiro" or ".cursor";
        }

        /// <summary>
        /// The anchor folder this instance landed in, and therefore which harness it is generating
        /// for. Every template is offered every AI.Context anchor, so landing somewhere this one
        /// does not serve is the normal case rather than an error - it declines silently.
        /// </summary>
        private string HarnessFolder => OutputTarget.Name;

        /// <summary>
        /// Each harness reads its hook config from its own folder, and runs the copy of the gate
        /// that sits beside it, so nothing reaches across into another harness's folder. The path
        /// is relative to the project root because that is the directory every one of these
        /// harnesses runs a hook command from - and unlike an absolute git-root path, it stays
        /// correct for an application nested below the repository root, such as a test app.
        /// </summary>
        private string GateCommand(string command) =>
            $"dotnet run {HarnessFolder}/hooks/gate/gate.cs --no-build -- {command} --harness {HarnessId}"
            + "; test $? -eq 0 && exit 0 || exit 2";

        private string WarmCommand() => $"dotnet run {HarnessFolder}/hooks/gate/gate.cs -- warm";

        private string HarnessId => HarnessFolder.TrimStart('.');

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // Written INSIDE the anchor, never escaping with "../" - an escaped path resolves the
            // same from every anchor, which is what previously collapsed three instances onto one
            // file and stopped the Software Factory dead. CanRunTemplate has already filtered out
            // anything not listed here; the fallback arm exists only so it can never throw.
            return HarnessFolder switch
            {
                ".codex" => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: ""),
                ".kiro" => new TemplateFileConfig(fileName: "intent-agent-gate", fileExtension: "json", relativeLocation: "hooks"),
                ".cursor" => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: ""),
                _ => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: ""),
            };
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // Each harness matches PreToolUse against its OWN internal tool names - there is no
            // shared vocabulary between them. Kiro's is "fs_write" per its hooks reference (grounded
            // in documentation, not observed: its CLI does not execute hook files yet). Codex's is
            // unverified and is the next to confirm. A wrong matcher fails silently and OPEN - the
            // config looks right and simply never matches - so these are the highest-value thing to
            // check the moment a harness will actually run a hook.
            return HarnessFolder switch
            {
                ".codex" => $$"""
                    {
                      "hooks": {
                        "SessionStart": [
                          { "hooks": [ { "type": "command", "command": "{{WarmCommand()}}" } ] }
                        ],
                        "PreToolUse": [
                          {
                            "matcher": "Write|Edit|ApplyPatch",
                            "hooks": [ { "type": "command", "command": "{{GateCommand("guard-write")}}" } ]
                          },
                          {
                            "matcher": ".*run_designer_script.*",
                            "hooks": [ { "type": "command", "command": "{{GateCommand("guard-version")}}" } ]
                          }
                        ],
                        "Stop": [
                          { "hooks": [ { "type": "command", "command": "{{GateCommand("close-out")}}" } ] }
                        ]
                      }
                    }
                    """,
                ".kiro" => $$"""
                    {
                      "version": "v1",
                      "hooks": [
                        {
                          "name": "intent-agent-gate-warm",
                          "trigger": "SessionStart",
                          "action": { "type": "command", "command": "{{WarmCommand()}}" }
                        },
                        {
                          "name": "intent-agent-gate-guard-write",
                          "trigger": "PreToolUse",
                          "matcher": "fs_write",
                          "action": { "type": "command", "command": "{{GateCommand("guard-write")}}" }
                        },
                        {
                          "name": "intent-agent-gate-guard-version",
                          "trigger": "PreToolUse",
                          "matcher": ".*run_designer_script.*",
                          "action": { "type": "command", "command": "{{GateCommand("guard-version")}}" }
                        },
                        {
                          "name": "intent-agent-gate-close-out",
                          "trigger": "Stop",
                          "action": { "type": "command", "command": "{{GateCommand("close-out")}}" }
                        }
                      ]
                    }
                    """,
                ".cursor" => $$"""
                    {
                      "version": 1,
                      "hooks": {
                        "sessionStart": [
                          { "command": "{{WarmCommand()}}" }
                        ],
                        "afterFileEdit": [
                          { "command": "{{GateCommand("guard-write")}}" }
                        ],
                        "beforeMCPExecution": [
                          { "command": "{{GateCommand("guard-version")}}", "matcher": ".*run_designer_script.*" }
                        ],
                        "stop": [
                          { "command": "{{GateCommand("close-out")}}" }
                        ]
                      }
                    }
                    """,
                _ => "{}",
            };
        }
    }
}