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
            // No "--scheme" is passed: the "Use Pre-release Versions" setting is baked into the gate's
            // own Cli.cs, so this command is spelled the same as every other harness's.
            //
            // close-out runs when the session goes idle, OpenCode's end of a turn - the counterpart of
            // Claude Code's Stop. OpenCode has no stop hook; "session.status" with status "idle" is the
            // current event and "session.idle" the deprecated one, still published, so both are heard
            // and a second notice within three seconds is dropped. It runs synchronously: "opencode run"
            // exits as soon as the session is idle, and an asynchronous child was killed before it
            // finished (observed). close-out never throws. Its warning goes to OpenCode's log and a
            // toast - to the person, as Claude Code's systemMessage does; putting it into the session
            // would start another turn.
            //
            // "patch" is OpenCode's apply_patch tool, offered to some models instead of write/edit; its
            // patch text is found by the gate by content, as for Codex.
            //
            // Kept inside the body deliberately: this member's signature is Mode.Fully, so a comment
            // above it is stripped on every regeneration. Body = Mode.Ignore is what protects it.
            return $$"""
                import { spawnSync } from "child_process";

                const gatePath = () => `${process.cwd()}/.opencode/hooks/gate/gate.cs`;
                const writeTools = new Set(["write", "edit", "patch", "multiedit"]);
                let lastCloseOut = 0;

                function runGuard(command: string, toolName: string, args: unknown): void {
                  const result = spawnSync(
                    "dotnet",
                    ["run", gatePath(), "--", command, "--harness", "opencode"],
                    { input: JSON.stringify({ tool_name: toolName, tool_input: args }), encoding: "utf-8" },
                  );

                  if (result.status !== 0) {
                    const reason = (result.stderr || "").trim();
                    throw new Error(reason || `intent-agent-gate blocked this action (exit ${result.status})`);
                  }
                }

                function runCloseOut(): string {
                  const result = spawnSync("dotnet", ["run", gatePath(), "--", "close-out", "--harness", "opencode"], {
                    stdio: ["ignore", "pipe", "ignore"],
                    encoding: "utf-8",
                  });
                  return (result.stdout || "").trim();
                }

                type ToolExecuteInput = { tool: string };
                type ToolExecuteOutput = { args: Record<string, unknown> };
                type PluginInput = { client?: any };

                export const IntentAgentGatePlugin = async ({ client }: PluginInput) => {
                  return {
                    "tool.execute.before": async (input: ToolExecuteInput, output: ToolExecuteOutput) => {
                      if (writeTools.has(input.tool)) {
                        runGuard("guard-write", input.tool, output.args);
                        return;
                      }

                      if (input.tool.includes("run_designer_script")) {
                        runGuard("guard-version", input.tool, output.args);
                      }
                    },
                    event: async ({ event }: { event: { type: string; properties?: any } }) => {
                      const idle = event.type === "session.idle"
                        || (event.type === "session.status" && event.properties?.status?.type === "idle");
                      if (!idle || Date.now() - lastCloseOut < 3000) {
                        return;
                      }

                      lastCloseOut = Date.now();
                      const warning = runCloseOut();
                      if (!warning) {
                        return;
                      }

                      try {
                        await client?.app?.log({ body: { service: "intent-agent-gate", level: "warn", message: warning } });
                      } catch {}
                      try {
                        await client?.tui?.showToast({ body: { message: warning, variant: "warning" } });
                      } catch {}
                    },
                  };
                };
                """;
        }
    }
}