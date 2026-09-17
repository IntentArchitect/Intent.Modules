using System.Xml.Linq;

namespace Intent.Agent.Gate;

/// <summary>
/// A path that was found listed as Software-Factory-owned generated output in a module's
/// "*.application.managed-files.xml".
/// </summary>
public sealed record ManagedFileMatch(string ManagedFilesXmlPath, string TemplateId, string ResolvedPath);

/// <summary>
/// Checks whether a file path is listed as generated output in any module's
/// "*.application.managed-files.xml". Each entry's "path" attribute is relative to the
/// directory containing that xml file (which may itself walk upward with "../", e.g. to
/// reach a shared "Modules/.claude/rules/..." file emitted by several modules' static
/// content templates).
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

        var baseDir = Path.GetDirectoryName(xmlPath)!;

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
}