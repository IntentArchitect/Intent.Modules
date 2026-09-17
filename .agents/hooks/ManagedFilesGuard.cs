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
    public static ManagedFileMatch? FindMatch(string repoRoot, string targetPath)
    {
        var targetFull = Path.GetFullPath(targetPath, repoRoot);

        var modulesDir = Path.Combine(repoRoot, "Modules");
        if (!Directory.Exists(modulesDir))
        {
            return null;
        }

        foreach (var xmlPath in Directory.EnumerateFiles(modulesDir, "*.application.managed-files.xml", SearchOption.AllDirectories))
        {
            var match = CheckFile(xmlPath, targetFull);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
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