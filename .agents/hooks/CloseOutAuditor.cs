using System.Xml.Linq;

namespace Intent.Agent.Gate;

public sealed record CloseOutFinding(string ModuleName, string Message);

/// <summary>
/// Close-out warn checks beyond "did the version move" (see ModuleVersionAuditor for that
/// one). Each of these turns on a judgment a script cannot make reliably - a docs folder
/// that genuinely doesn't need updating this time, a tag list that's deliberately short -
/// so every one of them warns rather than denies; see the module's plan document for why.
/// </summary>
public static class CloseOutAuditor
{
    public static IReadOnlyList<CloseOutFinding> Audit(string repoRoot, IReadOnlyList<string> changedFiles)
    {
        var normalizedChanged = changedFiles.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var findings = new List<CloseOutFinding>();
        var modulesDir = Path.Combine(repoRoot, "Modules");

        foreach (var moduleName in TouchedModules(normalizedChanged))
        {
            var moduleDir = Path.Combine(modulesDir, moduleName);
            if (!Directory.Exists(moduleDir))
            {
                continue;
            }

            var imodspecPath = Directory.EnumerateFiles(moduleDir, "*.imodspec", SearchOption.AllDirectories).FirstOrDefault();
            if (imodspecPath is null)
            {
                // Not an Intent module - nothing here to check.
                continue;
            }

            var imodspecContent = TryRead(imodspecPath);

            CheckTags(moduleName, imodspecContent, findings);
            CheckReadme(moduleName, moduleDir, findings);
            CheckContext(moduleName, moduleDir, repoRoot, normalizedChanged, findings);
            CheckReleaseNotesHeading(moduleName, moduleDir, imodspecContent, findings);
        }

        return findings;
    }

    private static IEnumerable<string> TouchedModules(IReadOnlySet<string> normalizedChanged)
    {
        var modules = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in normalizedChanged)
        {
            const string prefix = "Modules/";
            if (!file.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var remainder = file[prefix.Length..];
            var slashIndex = remainder.IndexOf('/');
            if (slashIndex > 0)
            {
                modules.Add(remainder[..slashIndex]);
            }
        }

        return modules;
    }

    private static void CheckTags(string moduleName, string? imodspecContent, List<CloseOutFinding> findings)
    {
        if (imodspecContent is null)
        {
            return;
        }

        string? tags;
        try
        {
            tags = (string?)XDocument.Parse(imodspecContent).Root?.Element("tags");
        }
        catch (Exception)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(tags))
        {
            return;
        }

        var badTags = tags.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(tag => tag != tag.ToLowerInvariant())
            .ToList();

        if (badTags.Count > 0)
        {
            findings.Add(new CloseOutFinding(moduleName,
                $"Tags '{string.Join(", ", badTags)}' are not lowercase - see module-docs-chore's tag format guidance."));
        }
    }

    private static void CheckReadme(string moduleName, string moduleDir, List<CloseOutFinding> findings)
    {
        if (!File.Exists(Path.Combine(moduleDir, "docs", "README.md")))
        {
            findings.Add(new CloseOutFinding(moduleName, "No docs/README.md - see module-docs-chore."));
        }
    }

    private static void CheckContext(string moduleName, string moduleDir, string repoRoot, IReadOnlySet<string> normalizedChanged, List<CloseOutFinding> findings)
    {
        var contextPath = Path.Combine(moduleDir, "CONTEXT.md");
        if (!File.Exists(contextPath))
        {
            // Absence is a deliberate choice per module-context-capture, not a finding here.
            return;
        }

        var relativeContext = Normalize(Path.GetRelativePath(repoRoot, contextPath));
        if (!normalizedChanged.Contains(relativeContext))
        {
            findings.Add(new CloseOutFinding(moduleName,
                "CONTEXT.md was not touched alongside this change - see module-context-capture. " +
                "If there is genuinely nothing durable to record, that is fine; say so rather than inventing an entry."));
        }
    }

    private static void CheckReleaseNotesHeading(string moduleName, string moduleDir, string? imodspecContent, List<CloseOutFinding> findings)
    {
        var releaseNotesPath = Path.Combine(moduleDir, "release-notes.md");
        if (!File.Exists(releaseNotesPath) || imodspecContent is null)
        {
            return;
        }

        string? version;
        try
        {
            version = (string?)XDocument.Parse(imodspecContent).Root?.Element("version");
        }
        catch (Exception)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(version) || !version.Contains("-pre", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var plainVersion = version[..version.IndexOf("-pre", StringComparison.OrdinalIgnoreCase)];
        string releaseNotes;
        try
        {
            releaseNotes = File.ReadAllText(releaseNotesPath);
        }
        catch (IOException)
        {
            return;
        }

        if (releaseNotes.Contains($"Version {version}", StringComparison.OrdinalIgnoreCase) &&
            !releaseNotes.Contains($"Version {plainVersion}", StringComparison.OrdinalIgnoreCase))
        {
            findings.Add(new CloseOutFinding(moduleName,
                $"release-notes.md heading reads 'Version {version}' - the -pre suffix belongs stripped in the " +
                $"heading ('Version {plainVersion}'); see module-docs-chore."));
        }
    }

    private static string? TryRead(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (IOException)
        {
            return null;
        }
    }

    private static string Normalize(string path) => path.Replace('\\', '/');
}