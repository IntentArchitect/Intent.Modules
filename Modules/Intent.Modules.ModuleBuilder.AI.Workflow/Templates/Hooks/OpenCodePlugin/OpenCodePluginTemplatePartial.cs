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

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.OpenCodePlugin
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class OpenCodePluginTemplate : IntentTemplateBase<IList<object>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.OpenCodePlugin";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public OpenCodePluginTemplate(IOutputTarget outputTarget, IList<object> model = null) : base(TemplateId, outputTarget, model)
        {
        }

        /// <summary>
        /// Only generates inside ".opencode". Every template is offered every AI.Context anchor, so
        /// landing anywhere else is the normal case and declines silently.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == ".opencode";
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            // Nested inside the anchor rather than escaping with "../" - see GateScripts for why an
            // escaped path collapses every anchor onto one file.
            return new TemplateFileConfig(fileName: "intent-agent-gate", fileExtension: "ts", relativeLocation: "plugins");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            // OpenCode's own tool.execute.before hook receives raw tool args directly (no JSON stdin
            // parsing needed on the gate's side beyond what it already tolerates) - confirmed by
            // directly probing a real opencode run rather than assumed from docs: "write"/"edit" for
            // the built-in file tools, args.filePath, args.content (write), args.oldString/newString
            // (edit, camelCase - not Claude Code's snake_case "new_string").
            //
            // Unlike the JSON-configured harnesses, this plugin controls process invocation directly
            // via Node's child_process, so it checks the exit code itself rather than needing the
            // shell-wrapper fail-closed trick HooksJson's commands rely on: any non-zero exit -
            // including a build failure - already throws here.
            //
            // Kept inside the body deliberately: this member's signature is Mode.Fully, so a comment
            // above it is stripped on every regeneration. Body = Mode.Ignore is what protects it.
            var settings = Intent.Modules.ModuleBuilder.AI.Workflow.Settings.ModuleSettingsExtensions.GetAIWorkflowSettings(ExecutionContext.Settings);
            var scheme = settings.UsePreReleaseVersions() ? "pre" : "final";

            return $$"""
                import { spawnSync } from "child_process";

                function runGate(command: string, extraArgs: string[], stdinPayload: unknown): void {
                  const gatePath = `${process.cwd()}/.opencode/hooks/gate/gate.cs`;
                  const result = spawnSync(
                    "dotnet",
                    ["run", gatePath, "--", command, "--harness", "opencode", ...extraArgs],
                    { input: JSON.stringify(stdinPayload), encoding: "utf-8" },
                  );

                  if (result.status !== 0) {
                    const reason = (result.stderr || "").trim();
                    throw new Error(reason || `intent-agent-gate blocked this action (exit ${result.status})`);
                  }
                }

                type ToolExecuteInput = { tool: string };
                type ToolExecuteOutput = { args: Record<string, unknown> };

                export const IntentAgentGatePlugin = async () => {
                  return {
                    "tool.execute.before": async (input: ToolExecuteInput, output: ToolExecuteOutput) => {
                      if (input.tool === "write" || input.tool === "edit") {
                        runGate("guard-write", [], { tool_input: output.args });
                        return;
                      }

                      if (input.tool.includes("run_designer_script")) {
                        runGate("guard-version", ["--scheme", "{{scheme}}"], { tool_input: output.args });
                      }
                    },
                  };
                };
                """;
        }
    }
}