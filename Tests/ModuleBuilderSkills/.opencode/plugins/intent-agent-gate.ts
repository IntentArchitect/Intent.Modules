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