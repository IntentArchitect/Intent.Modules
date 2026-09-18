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
        /// Claude Code and OpenCode are deliberately absent from the harness switches below, not
        /// overlooked. Claude Code's ".claude/settings.json" is shared with the developer's own
        /// permissions and environment, so it needs merge-aware generation this module does not
        /// do - it ships as manual-paste instructions in CLAUDE_SETUP.md instead. OpenCode
        /// enforces through a TypeScript plugin (see OpenCodePlugin), not a JSON hook config.
        /// An unrecognised harness is skipped with a warning rather than an exception: a harness
        /// this module has never heard of must never fail a consumer's Software Factory run.
        /// </summary>
        public override bool CanRunTemplate()
        {
            if (!base.CanRunTemplate())
            {
                return false;
            }

            if (Model.Harness is "codex" or "kiro" or "cursor")
            {
                return true;
            }

            Logging.Log.Warning(
                $"{TemplateId}: no hook-config shape is known for harness '{Model.Harness}', so no hook " +
                "config was generated for it. That harness is left unguarded by the agent gate; every " +
                "other harness in this repository is unaffected.");
            return false;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // CanRunTemplate has already filtered out anything not listed here, so the fallback
            // arm is unreachable - it exists only so an unknown harness can never throw.
            return Model.Harness switch
            {
                "codex" => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: $"../{Model.FolderName}"),
                "kiro" => new TemplateFileConfig(fileName: "intent-agent-gate", fileExtension: "json", relativeLocation: $"../{Model.FolderName}/hooks"),
                "cursor" => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: $"../{Model.FolderName}"),
                _ => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: $"../{Model.FolderName}"),
            };
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // As above: unreachable, and inert rather than fatal if it ever is reached.
            return Model.Harness switch
            {
                "codex" => """
                    {
                      "hooks": {
                        "SessionStart": [
                          {
                            "hooks": [
                              { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" -- warm" }
                            ]
                          }
                        ],
                        "PreToolUse": [
                          {
                            "matcher": "Write|Edit|ApplyPatch",
                            "hooks": [
                              { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" --no-build -- guard-write --harness codex; test $? -eq 0 && exit 0 || exit 2" }
                            ]
                          },
                          {
                            "matcher": ".*run_designer_script.*",
                            "hooks": [
                              { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" --no-build -- guard-version --harness codex; test $? -eq 0 && exit 0 || exit 2" }
                            ]
                          }
                        ],
                        "Stop": [
                          {
                            "hooks": [
                              { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" --no-build -- close-out --harness codex; test $? -eq 0 && exit 0 || exit 2" }
                            ]
                          }
                        ]
                      }
                    }
                    """,
                "kiro" => """
                    {
                      "version": "v1",
                      "hooks": [
                        {
                          "name": "intent-agent-gate-warm",
                          "trigger": "SessionStart",
                          "action": { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" -- warm" }
                        },
                        {
                          "name": "intent-agent-gate-guard-write",
                          "trigger": "PreToolUse",
                          "matcher": "Write|Edit|ApplyPatch",
                          "action": { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" --no-build -- guard-write --harness kiro; test $? -eq 0 && exit 0 || exit 2" }
                        },
                        {
                          "name": "intent-agent-gate-guard-version",
                          "trigger": "PreToolUse",
                          "matcher": ".*run_designer_script.*",
                          "action": { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" --no-build -- guard-version --harness kiro; test $? -eq 0 && exit 0 || exit 2" }
                        },
                        {
                          "name": "intent-agent-gate-close-out",
                          "trigger": "Stop",
                          "action": { "type": "command", "command": "dotnet run \"$(git rev-parse --show-toplevel)/.agents/hooks/gate.cs\" --no-build -- close-out --harness kiro; test $? -eq 0 && exit 0 || exit 2" }
                        }
                      ]
                    }
                    """,
                "cursor" => """
                    {
                      "version": 1,
                      "hooks": {
                        "sessionStart": [
                          { "command": "dotnet run .agents/hooks/gate.cs -- warm" }
                        ],
                        "afterFileEdit": [
                          { "command": "dotnet run .agents/hooks/gate.cs --no-build -- guard-write --harness cursor; test $? -eq 0 && exit 0 || exit 2" }
                        ],
                        "beforeMCPExecution": [
                          { "command": "dotnet run .agents/hooks/gate.cs --no-build -- guard-version --harness cursor; test $? -eq 0 && exit 0 || exit 2", "matcher": ".*run_designer_script.*" }
                        ],
                        "stop": [
                          { "command": "dotnet run .agents/hooks/gate.cs --no-build -- close-out --harness cursor; test $? -eq 0 && exit 0 || exit 2" }
                        ]
                      }
                    }
                    """,
                _ => "{}",
            };
        }
    }
}