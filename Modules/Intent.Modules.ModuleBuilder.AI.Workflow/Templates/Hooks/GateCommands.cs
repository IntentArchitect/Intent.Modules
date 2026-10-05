namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// The command strings every hook config invokes the gate with. Shared because each harness gets
    /// its own template - the configs differ, the commands do not - and six hand-maintained copies of
    /// the same string is how one harness silently ends up on a stale contract.
    /// </summary>
    internal static class GateCommands
    {
        /// <summary>
        /// Each harness runs the copy of the gate nested in its own folder, so nothing reaches across
        /// into another harness's directory.
        /// </summary>
        /// <remarks>
        /// A plain path relative to the project root, which is what every one of these harnesses
        /// documents for its own hook commands - Cursor's reference uses "./hooks/validate-tool.sh",
        /// Copilot's uses "./scripts/log-prompt.sh". Identical in form across all six, with no
        /// harness-specific variable in any of them.
        /// <para>
        /// Relative to the PROJECT root rather than the repository root: unlike an absolute git-root
        /// path it stays correct for an application nested below the repository root, such as a test
        /// app, and unlike an absolute path baked in at generation time it survives the repository
        /// being cloned or moved - which matters because these files are committed.
        /// </para>
        /// <para>
        /// KNOWN HAZARD, deliberately not solved here: a hook command runs in whatever the agent's
        /// current directory happens to be, so if the agent leaves the project root - running a build
        /// from inside a module folder is enough - the path stops resolving and the gate can no longer
        /// be found. It fails CLOSED, so nothing is waved through, but the agent is locked out of
        /// editing until its working directory returns. This is an agent-behaviour problem and
        /// belongs in agent guidance, not in every hook config: an earlier attempt to solve it in the
        /// config used Claude Code's "${CLAUDE_PROJECT_DIR}" placeholder, which worked but left one
        /// harness spelled differently from the other five, for a hazard none of them can solve
        /// uniformly.
        /// </para>
        /// </remarks>
        public static string GatePath(string harnessFolder) => $"{harnessFolder}/hooks/gate/gate.cs";

        /// <summary>The harness id the gate expects - the anchor folder name without its leading dot.</summary>
        public static string HarnessId(string harnessFolder) => harnessFolder.TrimStart('.');

        /// <summary>
        /// Builds the gate ahead of first use. Deliberately without --no-build: this is the run that
        /// produces what every later --no-build invocation depends on.
        /// </summary>
        public static string Warm(string harnessFolder) => $"dotnet run {GatePath(harnessFolder)} -- warm";

        /// <summary>
        /// A guard command, wrapped so anything other than a clean exit blocks.
        /// </summary>
        /// <remarks>
        /// The trailing wrapper is not decoration. Exit code 2 is the universal "block" signal, but a
        /// failure inside dotnet itself - a build error, a missing SDK - exits 1, which several
        /// harnesses treat as "allow". Collapsing every non-zero exit to 2 makes the gate fail closed
        /// rather than waving the action through on its own malfunction. Cursor in particular fails
        /// OPEN on any code that is not exactly 0 or 2. "$?" here is POSIX shell, not an environment
        /// variable.
        /// </remarks>
        public static string Guard(string harnessFolder, string command) =>
            $"dotnet run {GatePath(harnessFolder)} --no-build -- {command} --harness {HarnessId(harnessFolder)}"
            + "; test $? -eq 0 && exit 0 || exit 2";

        /// <summary>
        /// Commands this module generated in an EARLIER version for the same hook. A merge-aware
        /// template can safely replace one of these, because an exact match means the text is ours
        /// rather than the developer's.
        /// </summary>
        /// <remarks>
        /// Needed because the merge only ever ADDS absent entries. Without it, any change to the
        /// command itself would reach only repositories that had never generated the file before, and
        /// every existing install would keep the old form indefinitely. It works in both directions -
        /// it carried the "${CLAUDE_PROJECT_DIR}" anchoring out to existing installs, and now carries
        /// its removal back again.
        /// <para>
        /// Deliberately an exact-match list rather than "anything mentioning our gate path": the
        /// latter would also swallow a command the developer had adjusted, which this template
        /// promises never to overwrite.
        /// </para>
        /// </remarks>
        public static string[] Superseded(string harnessFolder, string command) => new[]
        {
            // Briefly anchored to Claude Code's project-root placeholder; removed so every harness is
            // spelled identically.
            $"dotnet run \"${{CLAUDE_PROJECT_DIR}}/{GatePath(harnessFolder)}\" --no-build -- {command} --harness {HarnessId(harnessFolder)}"
            + "; test $? -eq 0 && exit 0 || exit 2",
        };

        /// <summary>The superseded forms of <see cref="Warm"/>, for the same reason.</summary>
        public static string[] SupersededWarm(string harnessFolder) => new[]
        {
            $"dotnet run \"${{CLAUDE_PROJECT_DIR}}/{GatePath(harnessFolder)}\" -- warm",
        };
    }
}
