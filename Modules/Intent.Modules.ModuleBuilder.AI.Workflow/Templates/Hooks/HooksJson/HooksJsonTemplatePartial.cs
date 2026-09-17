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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return Model.Harness switch
            {
                "codex" => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: $"../{Model.FolderName}"),
                "kiro" => new TemplateFileConfig(fileName: "intent-agent-gate", fileExtension: "json", relativeLocation: $"../{Model.FolderName}/hooks"),
                "cursor" => new TemplateFileConfig(fileName: "hooks", fileExtension: "json", relativeLocation: $"../{Model.FolderName}"),
                _ => throw new InvalidOperationException($"Unsupported harness: {Model.Harness}"),
            };
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
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
                _ => throw new InvalidOperationException($"Unsupported harness: {Model.Harness}"),
            };
        }
    }
}