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
        /// Relative to the project root, not the repository root: this is the directory these
        /// harnesses launch hook commands from, and unlike an absolute git-root path it stays correct
        /// for an application nested below the repository root, such as a test app.
        /// <para>
        /// KNOWN LIMITATION: because it is relative, it stops resolving if the agent's working
        /// directory moves into a subdirectory - running a build inside a module folder is enough.
        /// The hook then fails closed, which is the safe direction, but the agent is locked out of
        /// editing with an error that names neither the gate nor the cause. Anchoring this to each
        /// harness's project-directory variable is the fix.
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
        /// OPEN on any code that is not exactly 0 or 2.
        /// </remarks>
        public static string Guard(string harnessFolder, string command) =>
            $"dotnet run {GatePath(harnessFolder)} --no-build -- {command} --harness {HarnessId(harnessFolder)}"
            + "; test $? -eq 0 && exit 0 || exit 2";
    }
}
