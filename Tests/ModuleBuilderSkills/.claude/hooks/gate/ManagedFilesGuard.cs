using System.Linq;
using System.Xml.Linq;

namespace Intent.Agent.Gate;

/// <summary>
/// A path that was found listed as Software-Factory-owned generated output in a module's
/// "*.application.managed-files.xml".
/// </summary>
public sealed record ManagedFileMatch(string ManagedFilesXmlPath, string TemplateId, string ResolvedPath);

/// <summary>
/// Checks whether a file path is listed as generated output in any module's
/// "*.application.managed-files.xml". Each entry's "path" attribute is relative to that
/// APPLICATION'S OWN OUTPUT ROOT - not necessarily the directory the xml file itself sits
/// in. Those are the same directory for an ordinary module (its own "location" attribute
/// on the sibling "*.application.config" is "."), but not for an app whose output root is
/// elsewhere - e.g. a dogfooding app whose metadata lives under "Modules/AppName/" while
/// its "location" attribute reads "../.." to reach the repo root it actually outputs to.
/// Found by a failing smoke test, not by inspection: a path that is genuinely managed
/// output was silently allowed because "location" was never consulted.
/// </summary>
public static class ManagedFilesGuard
{
    private static readonly string[] SkipDirectories =
    {
        ".git", ".vs", ".cache", ".intent", "node_modules", "bin", "obj", "nuget-packages",
    };

    /// <summary>
    /// Files the Software Factory MERGES into rather than owns outright. They appear in
    /// managed-files.xml like any other generated output, but the developer co-owns them
    /// and edits them routinely - ".claude/settings.json" also carries their permissions,
    /// environment and unrelated hooks. Denying edits to those would make the gate
    /// actively obstructive, so they are exempt: regeneration adds only what is missing
    /// and never overwrites what is already there.
    /// </summary>
    private static readonly string[] MergeOwnedSuffixes =
    {
        Path.Combine(".claude", "settings.json"),
    };

    /// <summary>
    /// Templates that SCAFFOLD a file rather than own its contents. The Module Builder
    /// emits a "*TemplatePartial.cs"/"*TemplateRegistration.cs" pair once and then the
    /// developer writes the template's actual logic into method bodies marked
    /// "Body = Mode.Ignore" or "Mode.Merge" - hand-editing those is the ONLY way to author
    /// a template, so denying it makes module development impossible under the gate.
    ///
    /// Members the designer does own (a "Mode.Fully" TemplateId, for instance) are still
    /// rewritten on the next run; that is covered by guidance in known-build-gotchas
    /// rather than by blocking the whole file, because the gate matches on paths and
    /// cannot see which member an edit touches.
    /// </summary>
    private static readonly string[] CoOwnedScaffoldTemplateIdPrefixes =
    {
        "Intent.ModuleBuilder.ProjectItemTemplate.",
        "Intent.ModuleBuilder.TemplateRegistration.",
    };

    private static bool IsCoOwnedScaffold(string templateId) =>
        CoOwnedScaffoldTemplateIdPrefixes.Any(prefix =>
            templateId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

    public static ManagedFileMatch? FindMatch(string repoRoot, string targetPath)
    {
        var targetFull = Path.GetFullPath(targetPath, repoRoot);

        if (MergeOwnedSuffixes.Any(suffix => targetFull.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        foreach (var xmlPath in EnumerateManifests(repoRoot))
        {
            var match = CheckFile(xmlPath, targetFull);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    /// <summary>
    /// Applications do not all live under "Modules/". A solution's test and sample
    /// applications commonly sit under "Tests/" instead - and those are exactly the ones
    /// an agent harness gets pointed at. Anchoring the scan on "Modules/" meant such an
    /// application's generated output matched nothing and every edit to it was allowed,
    /// which reads as a passing test while protecting nothing at all.
    /// </summary>
    private static IEnumerable<string> EnumerateManifests(string repoRoot)
    {
        var pending = new Stack<string>();
        pending.Push(repoRoot);

        while (pending.Count > 0)
        {
            var directory = pending.Pop();

            string[] files;
            string[] subDirectories;
            try
            {
                files = Directory.GetFiles(directory, "*.application.managed-files.xml");
                subDirectories = Directory.GetDirectories(directory);
            }
            catch (Exception)
            {
                // An unreadable directory must not take the whole gate down.
                continue;
            }

            foreach (var file in files)
            {
                yield return file;
            }

            foreach (var subDirectory in subDirectories)
            {
                var name = Path.GetFileName(subDirectory);
                if (!SkipDirectories.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    pending.Push(subDirectory);
                }
            }
        }
    }

    private static ManagedFileMatch? CheckFile(string xmlPath, string targetFull)
    {
        XDocument document;
        try
        {
            document = XDocument.Load(xmlPath);
        }
        catch (Exception)
        {
            // A malformed managed-files.xml should not crash the gate - treat as no match.
            return null;
        }

        var baseDir = ResolveApplicationOutputRoot(xmlPath);

        foreach (var fileElement in document.Descendants("file"))
        {
            var relativePath = (string?)fileElement.Attribute("path");
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                continue;
            }

            string resolved;
            try
            {
                resolved = Path.GetFullPath(Path.Combine(baseDir, relativePath));
            }
            catch (Exception)
            {
                continue;
            }

            if (string.Equals(resolved, targetFull, StringComparison.OrdinalIgnoreCase))
            {
                var templateId = (string?)fileElement.Attribute("templateId") ?? "(unknown template)";
                if (IsCoOwnedScaffold(templateId))
                {
                    continue;
                }

                return new ManagedFileMatch(xmlPath, templateId, resolved);
            }
        }

        return null;
    }

    private static string ResolveApplicationOutputRoot(string managedFilesXmlPath)
    {
        var metadataDir = Path.GetDirectoryName(managedFilesXmlPath)!;

        var configPath = Directory.EnumerateFiles(metadataDir, "*.application.config", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (configPath is null)
        {
            return metadataDir;
        }

        try
        {
            var location = (string?)XDocument.Load(configPath).Root?.Attribute("location");
            return string.IsNullOrWhiteSpace(location)
                ? metadataDir
                : Path.GetFullPath(Path.Combine(metadataDir, location));
        }
        catch (Exception)
        {
            return metadataDir;
        }
    }
}