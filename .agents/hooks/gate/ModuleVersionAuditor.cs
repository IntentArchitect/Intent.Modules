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
        var normalizedChanged = changedFiles
            .Select(Normalize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var modulesTouched = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in normalizedChanged)
        {
            var moduleName = TryGetModuleName(file);
            if (moduleName is not null)
            {
                modulesTouched.Add(moduleName);
            }
        }

        var violations = new List<ModuleVersionViolation>();
        var modulesDir = Path.Combine(repoRoot, "Modules");

        foreach (var moduleName in modulesTouched)
        {
            var moduleDir = Path.Combine(modulesDir, moduleName);
            if (!Directory.Exists(moduleDir))
            {
                continue;
            }

            var imodspecFiles = Directory.EnumerateFiles(moduleDir, "*.imodspec", SearchOption.AllDirectories).ToList();
            if (imodspecFiles.Count == 0)
            {
                // Not an Intent module (e.g. a shared non-module folder like "Modules/.claude")
                // - nothing to verify a version against.
                continue;
            }

            var imodspecRelativePaths = imodspecFiles
                .Select(spec => Normalize(Path.GetRelativePath(repoRoot, spec)))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var hasNonImodspecChange = normalizedChanged.Any(f =>
                IsUnderModule(f, moduleName) && !imodspecRelativePaths.Contains(f));

            if (!hasNonImodspecChange)
            {
                continue;
            }

            var imodspecChanged = imodspecRelativePaths.Overlaps(normalizedChanged);
            if (!imodspecChanged)
            {
                violations.Add(new ModuleVersionViolation(moduleName, imodspecRelativePaths.First()));
            }
        }

        return new ModuleVersionAuditResult(violations.Count == 0, violations);
    }

    private static bool IsUnderModule(string normalizedRelativePath, string moduleName)
    {
        var prefix = $"Modules/{moduleName}/";
        return normalizedRelativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    private static string? TryGetModuleName(string normalizedRelativePath)
    {
        const string prefix = "Modules/";
        if (!normalizedRelativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var remainder = normalizedRelativePath[prefix.Length..];
        var slashIndex = remainder.IndexOf('/');
        if (slashIndex <= 0)
        {
            // A file directly under "Modules/" (not inside a module subfolder) is not
            // itself a module.
            return null;
        }

        return remainder[..slashIndex];
    }

    private static string Normalize(string path) => path.Replace('\\', '/');
}