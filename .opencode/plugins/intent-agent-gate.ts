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
        runGate("guard-version", ["--scheme", "pre"], { tool_input: output.args });
      }
    },
  };
};