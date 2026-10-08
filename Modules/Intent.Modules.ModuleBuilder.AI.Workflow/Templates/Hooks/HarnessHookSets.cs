using System.Text.Json.Nodes;

#nullable enable

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// What this module registers in each MERGED hook file - one method per harness, each applied to
    /// whatever the file already holds.
    /// </summary>
    /// <remarks>
    /// Separate from the templates, and free of any Intent dependency, so the test project compiles
    /// this exact file: the tests then check the registrations that actually ship, not a restatement
    /// of them.
    /// </remarks>
    internal static class HarnessHookSets
    {
        /// <summary>".claude/settings.json". Claude Code runs hooks in bash - Git Bash on Windows.</summary>
        public static void MergeClaudeSettings(JsonObject root)
        {
            const string folder = ".claude";
            var gatePath = GateCommands.GatePath(folder);
            var hooks = HookConfigMerge.EnsureObject(root, "hooks");

            HookConfigMerge.EnsureGroupedHook(hooks, "SessionStart", null, gatePath, "warm",
                Command(GateCommands.Warm(folder)), GateCommands.SupersededWarm(folder));
            HookConfigMerge.EnsureGroupedHook(hooks, "PreToolUse", "Write|Edit", gatePath, "guard-write",
                Command(GateCommands.GuardPosix(folder, "guard-write")), GateCommands.Superseded(folder, "guard-write"));
            HookConfigMerge.EnsureGroupedHook(hooks, "PreToolUse", ".*run_designer_script.*", gatePath, "guard-version",
                Command(GateCommands.GuardPosix(folder, "guard-version")), GateCommands.Superseded(folder, "guard-version"));
            HookConfigMerge.EnsureGroupedHook(hooks, "Stop", null, gatePath, "close-out",
                Command(GateCommands.CloseOut(folder)), GateCommands.Superseded(folder, "close-out"));
        }

        /// <summary>
        /// ".codex/hooks.json". Codex runs "command" in a POSIX shell and "commandWindows" in
        /// PowerShell on Windows - observed, the docs do not name the shell.
        /// </summary>
        /// <remarks>
        /// The write matcher is "apply_patch|Edit|Write": Codex edits files through apply_patch, and
        /// its docs accept "Edit" and "Write" as aliases for it. The earlier "Write|Edit|ApplyPatch"
        /// was carried over from Claude Code's names unverified; its entry is found by command and
        /// rewritten with the new matcher.
        /// </remarks>
        public static void MergeCodexHooks(JsonObject root)
        {
            const string folder = ".codex";
            var gatePath = GateCommands.GatePath(folder);
            var hooks = HookConfigMerge.EnsureObject(root, "hooks");

            HookConfigMerge.EnsureGroupedHook(hooks, "SessionStart", null, gatePath, "warm",
                Command(GateCommands.Warm(folder)), GateCommands.SupersededWarm(folder));
            HookConfigMerge.EnsureGroupedHook(hooks, "PreToolUse", "apply_patch|Edit|Write", gatePath, "guard-write",
                Guard(folder, "guard-write"), GateCommands.Superseded(folder, "guard-write"));
            HookConfigMerge.EnsureGroupedHook(hooks, "PreToolUse", ".*run_designer_script.*", gatePath, "guard-version",
                Guard(folder, "guard-version"), GateCommands.Superseded(folder, "guard-version"));
            HookConfigMerge.EnsureGroupedHook(hooks, "Stop", null, gatePath, "close-out",
                Command(GateCommands.CloseOut(folder)), GateCommands.Superseded(folder, "close-out"));

            static JsonObject Guard(string folder, string command)
            {
                var hook = Command(GateCommands.GuardPosix(folder, command));
                hook["commandWindows"] = GateCommands.GuardPowerShell(folder, command);
                return hook;
            }
        }

        /// <summary>
        /// ".cursor/hooks.json" - Cursor allows exactly one project hooks file, so it is always shared.
        /// </summary>
        /// <remarks>
        /// guard-write runs on "preToolUse", not "afterFileEdit": Cursor has no before-file-edit hook,
        /// and afterFileEdit fires once the write has landed, so it could never deny anything. The
        /// preToolUse matcher takes Cursor's own tool categories (Write, Delete, Shell, MCP:&lt;name&gt;),
        /// not another harness's tool names.
        /// <para>
        /// Cursor's shell on Windows is undocumented, so the guards use the portable form, and
        /// "failClosed": true supplies what that form cannot: Cursor otherwise lets the action through
        /// when a hook exits with anything but 0 or 2, so a gate that could not run would wave writes
        /// through. sessionStart and stop are fire-and-forget and cannot block, so neither carries it.
        /// </para>
        /// </remarks>
        public static void MergeCursorHooks(JsonObject root)
        {
            const string folder = ".cursor";
            var gatePath = GateCommands.GatePath(folder);
            if (root["version"] is null)
            {
                root["version"] = 1;
            }

            var hooks = HookConfigMerge.EnsureObject(root, "hooks");

            HookConfigMerge.EnsureFlatHook(hooks, "sessionStart", gatePath, "warm",
                Command(GateCommands.Warm(folder)), GateCommands.SupersededWarm(folder));
            HookConfigMerge.EnsureFlatHook(hooks, "preToolUse", gatePath, "guard-write",
                FailClosed(GateCommands.GuardPortable(folder, "guard-write"), "Write|Delete"),
                GateCommands.Superseded(folder, "guard-write"));
            HookConfigMerge.EnsureFlatHook(hooks, "beforeMCPExecution", gatePath, "guard-version",
                FailClosed(GateCommands.GuardPortable(folder, "guard-version"), ".*run_designer_script.*"),
                GateCommands.Superseded(folder, "guard-version"));
            HookConfigMerge.EnsureFlatHook(hooks, "stop", gatePath, "close-out",
                Command(GateCommands.CloseOut(folder)), GateCommands.Superseded(folder, "close-out"));

            static JsonObject FailClosed(string command, string matcher)
            {
                var hook = Command(command);
                hook["matcher"] = matcher;
                hook["failClosed"] = true;
                return hook;
            }
        }

        private static JsonObject Command(string command) => new()
        {
            ["type"] = "command",
            ["command"] = command,
        };
    }
}
