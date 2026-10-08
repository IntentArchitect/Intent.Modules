namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// The command strings every hook config invokes the gate with. Shared because each harness gets
    /// its own template - the configs differ, the commands do not - and six hand-maintained copies of
    /// the same string is how one harness silently ends up on a stale contract.
    /// </summary>
    /// <remarks>
    /// There is no single command form that works everywhere, because the harnesses do not agree on
    /// a shell. Measured on Windows by driving each harness for real: Claude Code runs hooks in Git
    /// Bash, while Codex (commandWindows), Copilot (powershell) and Kiro run them in PowerShell. On
    /// macOS and Linux every harness uses a POSIX shell. So each form below is chosen per harness
    /// rather than shared:
    /// <list type="bullet">
    /// <item><see cref="GuardPosix"/> and <see cref="GuardPowerShell"/> - fail closed, for harnesses
    /// that let a hook pick its shell (Codex's commandWindows, Copilot's bash/powershell keys) and for
    /// Claude Code, which uses Git Bash on Windows.</item>
    /// <item><see cref="GuardPortable"/> - for harnesses with one command string and no say over the
    /// shell (Cursor, Kiro). Parses identically in bash and PowerShell.</item>
    /// <item><see cref="CloseOut"/> - never wrapped, on any harness.</item>
    /// </list>
    /// </remarks>
    internal static class GateCommands
    {
        /// <summary>
        /// Each harness runs the copy of the gate nested in its own folder, so nothing reaches across
        /// into another harness's directory.
        /// </summary>
        /// <remarks>
        /// A plain path relative to the project root, which is what every one of these harnesses
        /// documents for its own hook commands - Cursor's reference uses "./hooks/validate-tool.sh",
        /// Copilot's uses "./scripts/log-prompt.sh".
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
        /// be found. This is an agent-behaviour problem and belongs in agent guidance (the generated
        /// workflow instructions name it), not in every hook config.
        /// </para>
        /// </remarks>
        public static string GatePath(string harnessFolder) => $"{harnessFolder}/hooks/gate/gate.cs";

        /// <summary>The harness id the gate expects - the anchor folder name without its leading dot.</summary>
        public static string HarnessId(string harnessFolder) => harnessFolder.TrimStart('.');

        /// <summary>
        /// Builds the gate ahead of first use. Deliberately without --no-build: this is the run that
        /// produces what every later --no-build invocation depends on. Contains no shell syntax, so it
        /// runs unchanged in every shell.
        /// </summary>
        public static string Warm(string harnessFolder) => $"dotnet run {GatePath(harnessFolder)} -- warm";

        private static string Run(string harnessFolder, string command) =>
            $"dotnet run {GatePath(harnessFolder)} --no-build -- {command} --harness {HarnessId(harnessFolder)}";

        /// <summary>
        /// A guard command for a POSIX shell, wrapped so anything other than a clean exit blocks.
        /// </summary>
        /// <remarks>
        /// Exit code 2 is the universal "block" signal, but a failure inside dotnet itself - a build
        /// error, a missing SDK - exits 1, which several harnesses treat as "allow". "|| exit 2"
        /// collapses every non-zero exit to 2, so the gate fails closed rather than waving the action
        /// through on its own malfunction.
        /// <para>
        /// Replaces "; test $? -eq 0 &amp;&amp; exit 0 || exit 2", which behaves identically in bash but
        /// breaks in PowerShell 7 - and Copilot also runs the hooks in ".claude/settings.json", under
        /// PowerShell on Windows, where the old form failed and Copilot (which denies on a failed
        /// hook) blocked every write. "|| exit 2" allows correctly there and still denies on any
        /// failure. Measured, not assumed.
        /// </para>
        /// </remarks>
        public static string GuardPosix(string harnessFolder, string command) =>
            $"{Run(harnessFolder, command)} || exit 2";

        /// <summary>
        /// The PowerShell spelling of <see cref="GuardPosix"/>, for a harness field that is only ever
        /// run by PowerShell (Codex's "commandWindows", Copilot's "powershell").
        /// </summary>
        public static string GuardPowerShell(string harnessFolder, string command) =>
            $"{Run(harnessFolder, command)}; if ($LASTEXITCODE -ne 0) {{ exit 2 }}";

        /// <summary>
        /// A guard command for a harness that offers one command string and runs it in whatever shell
        /// the platform gives it - PowerShell on Windows, a POSIX shell elsewhere.
        /// </summary>
        /// <remarks>
        /// PowerShell does not pass a native program's exit code through: the gate's deliberate exit
        /// 2 reaches the harness as 1. Kiro treats 1 as a failed hook and lets the write through -
        /// observed, and the likely cause of Kiro's open "exit 2 does not block on Windows" issue.
        /// "; exit $LASTEXITCODE" restores the real code in PowerShell, and in a POSIX shell the
        /// variable is empty, so "exit" keeps the gate's own status. It does not fail closed when
        /// dotnet itself cannot run; Cursor covers that with failClosed, and Kiro has no equivalent.
        /// </remarks>
        public static string GuardPortable(string harnessFolder, string command) =>
            $"{Run(harnessFolder, command)}; exit $LASTEXITCODE";

        /// <summary>
        /// Close-out, never wrapped. A Stop hook that exits 2 tells Claude Code and Codex the agent
        /// must keep going, so a fail-closed wrapper turned a gate that could not run into an endless
        /// loop of turns. close-out never denies; when it cannot run, the end of the turn must still
        /// be allowed. No shell syntax, so it runs unchanged in every shell.
        /// </summary>
        public static string CloseOut(string harnessFolder) => Run(harnessFolder, "close-out");

        /// <summary>
        /// Commands this module generated in an EARLIER version for the same hook. A merge-aware
        /// template can safely replace one of these, because an exact match means the text is ours
        /// rather than the developer's.
        /// </summary>
        /// <remarks>
        /// Needed because the merge only ever ADDS absent entries. Without it, any change to the
        /// command itself would reach only repositories that had never generated the file before, and
        /// every existing install would keep the old form indefinitely.
        /// <para>
        /// Deliberately an exact-match list rather than "anything mentioning our gate path": the
        /// latter would also swallow a command the developer had adjusted, which these templates
        /// promise never to overwrite.
        /// </para>
        /// </remarks>
        public static string[] Superseded(string harnessFolder, string command) => new[]
        {
            // The shared POSIX wrapper every harness used up to 1.0.0-pre.8, close-out included.
            $"{Run(harnessFolder, command)}; test $? -eq 0 && exit 0 || exit 2",
            // Briefly anchored to Claude Code's project-root placeholder.
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
