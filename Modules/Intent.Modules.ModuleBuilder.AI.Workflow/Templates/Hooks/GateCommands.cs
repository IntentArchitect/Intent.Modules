namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// The command strings every hook config invokes the gate with. Shared because each harness gets
    /// its own template - the configs differ, the commands do not - and five hand-maintained copies
    /// of the same string is how one harness silently ends up on a stale contract.
    /// </summary>
    internal static class GateCommands
    {
        /// <summary>
        /// Each harness runs the copy of the gate nested in its own folder, so nothing reaches across
        /// into another harness's directory.
        /// </summary>
        /// <remarks>
        /// Relative to the project root, not the repository root: unlike an absolute git-root path it
        /// stays correct for an application nested below the repository root, such as a test app.
        /// <para>
        /// A bare relative path is NOT enough on its own. Hook commands do not run in a guaranteed
        /// project root - Claude Code's own documentation states the working directory is "the new
        /// directory after Claude runs cd" - so a plain "dotnet run .claude/hooks/gate/gate.cs"
        /// stops resolving the moment the agent's working directory moves into a subdirectory.
        /// Running a build inside a module folder is enough to trigger it, and it locks the agent
        /// out of editing entirely behind an error that names neither the gate nor the cause.
        /// Observed live, three times, while building this module.
        /// </para>
        /// <para>
        /// Only the LOOKUP of gate.cs is affected. Once running, the gate locates the repository
        /// root itself by walking up from the current directory, so it never mistakes a
        /// subdirectory for the root and never fails open on this.
        /// </para>
        /// </remarks>
        public static string GatePath(string harnessFolder) => $"{harnessFolder}/hooks/gate/gate.cs";

        /// <summary>
        /// The gate path as a hook command should spell it: anchored to the harness's own
        /// project-root variable where that harness defines one, so it resolves from any working
        /// directory.
        /// </summary>
        /// <remarks>
        /// Claude Code documents "${CLAUDE_PROJECT_DIR}" as the project root where the session
        /// started, and explicitly recommends it for referencing hook scripts "regardless of the
        /// working directory when the hook runs"; in shell form it must be double-quoted.
        /// <para>
        /// The other harnesses fall back to the bare relative path. That is not a judgement that
        /// they are immune - it is that no equivalent variable has been CONFIRMED for them, and
        /// inventing one would produce a command that silently expands to nothing and breaks the
        /// hook outright. Confirming those variables is outstanding work.
        /// </para>
        /// </remarks>
        public static string QuotedGatePath(string harnessFolder) => harnessFolder switch
        {
            ".claude" => $"\"${{CLAUDE_PROJECT_DIR}}/{GatePath(harnessFolder)}\"",
            _ => GatePath(harnessFolder),
        };

        /// <summary>The harness id the gate expects - the anchor folder name without its leading dot.</summary>
        public static string HarnessId(string harnessFolder) => harnessFolder.TrimStart('.');

        /// <summary>
        /// Builds the gate ahead of first use. Deliberately without --no-build: this is the run that
        /// produces what every later --no-build invocation depends on.
        /// </summary>
        public static string Warm(string harnessFolder) => $"dotnet run {QuotedGatePath(harnessFolder)} -- warm";

        /// <summary>
        /// A guard command, wrapped so anything other than a clean exit blocks.
        /// </summary>
        /// <remarks>
        /// The trailing wrapper is not decoration. Exit code 2 is the universal "block" signal, but a
        /// failure inside dotnet itself - a build error, a missing SDK - exits 1, which several
        /// harnesses treat as "allow". Collapsing every non-zero exit to 2 makes the gate fail closed
        /// rather than waving the action through on its own malfunction. Cursor in particular fails
        /// OPEN on any code that is not exactly 0 or 2.
        /// </remarks>
        /// <summary>
        /// Commands this module generated in an EARLIER version for the same hook. A merge-aware
        /// template can safely replace one of these, because an exact match means the text is ours
        /// rather than the developer's.
        /// </summary>
        /// <remarks>
        /// Needed because the merge only ever ADDS absent entries. Without this, a fix to the command
        /// itself — such as anchoring the gate path to "${CLAUDE_PROJECT_DIR}" — would reach only
        /// repositories that had never generated the file before, and every existing install would
        /// silently keep the broken form forever.
        /// <para>
        /// Deliberately an exact-match list rather than "anything mentioning our gate path": the
        /// latter would also swallow a command the developer had adjusted, which this template
        /// promises never to overwrite.
        /// </para>
        /// </remarks>
        public static string[] Superseded(string harnessFolder, string command) => new[]
        {
            // Before the gate path was anchored to the harness's project-root variable.
            $"dotnet run {GatePath(harnessFolder)} --no-build -- {command} --harness {HarnessId(harnessFolder)}"
            + "; test $? -eq 0 && exit 0 || exit 2",
        };

        /// <summary>The superseded forms of <see cref="Warm"/>, for the same reason.</summary>
        public static string[] SupersededWarm(string harnessFolder) => new[]
        {
            $"dotnet run {GatePath(harnessFolder)} -- warm",
        };

        public static string Guard(string harnessFolder, string command) =>
            $"dotnet run {QuotedGatePath(harnessFolder)} --no-build -- {command} --harness {HarnessId(harnessFolder)}"
            + "; test $? -eq 0 && exit 0 || exit 2";
    }
}
