namespace Intent.Agent.Gate;

/// <summary>
/// A module under "Modules/&lt;ModuleName&gt;/" whose files changed without its .imodspec
/// (the file carrying its version) also changing.
/// </summary>
public sealed record ModuleVersionViolation(string ModuleName, string ImodspecRelativePath);

public sealed record ModuleVersionAuditResult(bool Passed, IReadOnlyList<ModuleVersionViolation> Violations);

/// <summary>
/// The close-out check: for every module whose files changed (relative to HEAD), did that
/// module's own .imodspec also change? This proves only that the version line moved, not
/// that the chosen component (major/minor/patch) was correct - that judgement needs a
/// module-feed query this gate deliberately does not attempt, which is exactly why this is a
/// warning rather than a deny.
/// </summary>
public static class ModuleVersionAuditor
{
    public static ModuleVersionAuditResult Audit(string repoRoot, IReadOnlyList<string> changedFiles)
    {
        var normalizedChanged = changedFiles.Select(Normalize).ToList();
        var byApplication = ModuleDiscovery.GroupChangedFilesByApplication(repoRoot, normalizedChanged);

        var violations = new List<ModuleVersionViolation>();

        foreach (var (application, files) in byApplication)
        {
            var imodspecPath = ModuleDiscovery.FindImodspecUnder(application.OutputRoot);
            if (imodspecPath is null)
            {
                // Not a module in its own right (e.g. a plain consuming application, or
                // one whose output root merely contains other applications' modules as
                // descendants) - nothing to verify a version against.
                continue;
            }

            var imodspecRelative = Normalize(Path.GetRelativePath(repoRoot, imodspecPath));
            var hasNonImodspecChange = files.Any(f => !string.Equals(f, imodspecRelative, StringComparison.OrdinalIgnoreCase));
            if (!hasNonImodspecChange)
            {
                continue;
            }

            var imodspecChanged = files.Contains(imodspecRelative, StringComparer.OrdinalIgnoreCase);
            if (!imodspecChanged)
            {
                var moduleName = ModuleLabel(application.OutputRoot);
                violations.Add(new ModuleVersionViolation(moduleName, imodspecRelative));
            }
        }

        return new ModuleVersionAuditResult(violations.Count == 0, violations);
    }

    /// <summary>
    /// A human-readable label for messages - the output root's own folder name, matching
    /// what earlier versions of this check reported before module resolution moved off a
    /// hardcoded "Modules/{name}/" path assumption.
    /// </summary>
    internal static string ModuleLabel(string outputRoot) =>
        Path.GetFileName(outputRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

    private static string Normalize(string path) => path.Replace('\\', '/');
}