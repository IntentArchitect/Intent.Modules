using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Intent.Agent.Gate;

public sealed record DesignerScriptInvocation(string? ApplicationId, string? Script);

/// <summary>
/// Extracts the applicationId and script text from a "run_designer_script"-shaped
/// PreToolUse payload on stdin (tool_input.applicationId / tool_input.script). Tolerant of
/// the payload not matching - a call to a different MCP tool, or a harness whose payload
/// shape isn't this one, is simply not a version-setting call.
/// </summary>
public static class DesignerScriptInputExtractor
{
    public static DesignerScriptInvocation Extract(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new DesignerScriptInvocation(null, null);
        }

        try
        {
            using var document = JsonDocument.Parse(input);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("tool_input", out var toolInput) &&
                toolInput.ValueKind == JsonValueKind.Object)
            {
                return new DesignerScriptInvocation(
                    TryGetString(toolInput, "applicationId"),
                    TryGetString(toolInput, "script"));
            }

            return new DesignerScriptInvocation(null, null);
        }
        catch (JsonException)
        {
            return new DesignerScriptInvocation(null, null);
        }
    }

    private static string? TryGetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
}

/// <summary>
/// Pulls the version string out of a designer script's ".setProperty("Version", "...")"
/// call. The only scriptable way to set a module's version (confirmed against the live
/// Module Builder designer script API) is
/// "pkg.ensureStereotype("Module Settings").setProperty("Version", "X")", so a script that
/// doesn't match this pattern isn't a version change at all.
/// </summary>
public static class VersionScriptExtractor
{
    private static readonly Regex VersionSetPattern = new(
        "setProperty\\(\\s*[\"']Version[\"']\\s*,\\s*[\"']([^\"']+)[\"']\\s*\\)",
        RegexOptions.Compiled);

    public static string? ExtractNewVersion(string script)
    {
        var match = VersionSetPattern.Match(script);
        return match.Success ? match.Groups[1].Value : null;
    }
}

/// <summary>
/// Extracts the edit payload from a Write/Edit-style tool_input: the whole new file
/// content (Write), or the new_string/old_string pair of a partial replacement (Edit).
/// Tolerant of neither being present - a tool this gate doesn't recognise just gets no
/// opinion on the edit's content, only its path.
/// </summary>
public sealed record EditPayload(string? WholeFileContent, string? EditSnippet);

public static class StdinEditExtractor
{
    public static EditPayload Extract(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new EditPayload(null, null);
        }

        try
        {
            using var document = JsonDocument.Parse(input);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("tool_input", out var toolInput) ||
                toolInput.ValueKind != JsonValueKind.Object)
            {
                return new EditPayload(null, null);
            }

            var wholeFile = TryGetString(toolInput, "content");
            // "new_string" (Claude Code / Codex convention) and "newString" (OpenCode's
            // own edit tool - confirmed by probing it directly) are the same concept
            // under two different naming conventions.
            var newString = TryGetString(toolInput, "new_string") ?? TryGetString(toolInput, "newString");
            return new EditPayload(wholeFile, newString);
        }
        catch (JsonException)
        {
            return new EditPayload(null, null);
        }
    }

    private static string? TryGetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
}

/// <summary>
/// The ".imodspec" is mostly Software-Factory-owned, but three edits are legitimate and
/// must stay possible: a version downgrade (the SF won't write one from the designer),
/// interop/dependency entries, and "&lt;tags&gt;". Everything else - notably "&lt;summary&gt;"
/// and "&lt;description&gt;" - is overwritten by the SF from Application Settings on every
/// run, discarding a manual edit without an error, so a hand-edit there is always a deny.
/// </summary>
public static class ImodspecFieldGuard
{
    private static readonly string[] DeniedFields = ["summary", "description"];

    /// <param name="currentDiskContent">
    /// The file's current content, when known - present for a whole-file Write (so this
    /// can diff old vs new element-by-element, the only way to catch a full-file rewrite
    /// that changes "&lt;summary&gt;" alongside an innocent "&lt;tags&gt;" edit) and null for a
    /// partial Edit, where only the changed snippet itself is available to inspect.
    /// </param>
    public static bool IsHandEditAllowed(string? currentDiskContent, string editedText)
    {
        if (currentDiskContent is not null)
        {
            foreach (var field in DeniedFields)
            {
                if (!string.Equals(ExtractElement(currentDiskContent, field), ExtractElement(editedText, field), StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        foreach (var field in DeniedFields)
        {
            if (editedText.Contains($"<{field}>", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static string? ExtractElement(string xml, string elementName)
    {
        try
        {
            return (string?)XDocument.Parse(xml).Root?.Element(elementName);
        }
        catch (Exception)
        {
            return null;
        }
    }
}

/// <summary>
/// Resolves an applicationId to the module folder it lives in, and reads that module's
/// version, by scanning "Modules/*/*.application.config" for a matching root "id" attribute.
/// </summary>
public static class ModuleResolver
{
    public static string? FindModuleFolderByApplicationId(string repoRoot, string applicationId)
    {
        var modulesDir = Path.Combine(repoRoot, "Modules");
        if (!Directory.Exists(modulesDir))
        {
            return null;
        }

        foreach (var configPath in Directory.EnumerateFiles(modulesDir, "*.application.config", SearchOption.AllDirectories))
        {
            string content;
            try
            {
                content = File.ReadAllText(configPath);
            }
            catch (IOException)
            {
                continue;
            }

            if (!content.Contains(applicationId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                var document = XDocument.Load(configPath);
                var idAttribute = (string?)document.Root?.Attribute("id");
                if (string.Equals(idAttribute, applicationId, StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetDirectoryName(configPath);
                }
            }
            catch (Exception)
            {
                // A malformed application.config should not crash the gate - keep looking.
            }
        }

        return null;
    }

    public static string? FindImodspec(string moduleFolder) =>
        Directory.EnumerateFiles(moduleFolder, "*.imodspec", SearchOption.TopDirectoryOnly).FirstOrDefault();

    public static string? ReadVersionFromImodspecFile(string imodspecPath)
    {
        try
        {
            return ReadVersionFromImodspecContent(File.ReadAllText(imodspecPath));
        }
        catch (IOException)
        {
            return null;
        }
    }

    public static string? ReadVersionFromImodspecContent(string content)
    {
        try
        {
            return (string?)XDocument.Parse(content).Root?.Element("version");
        }
        catch (Exception)
        {
            return null;
        }
    }
}