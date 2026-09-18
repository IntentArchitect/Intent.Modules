namespace Intent.Agent.Gate;

/// <summary>
/// Intent Architect's own METADATA: the designer model, an application's configuration, and
/// the record of what it generated and installed. None of it is hand-edited.
/// </summary>
/// <remarks>
/// This is a different category from generated OUTPUT, and the distinction is the whole
/// point of the guard. Editing generated output is usually merely futile - the next run
/// overwrites it. Editing metadata CORRUPTS state the Software Factory depends on, and no
/// regeneration puts it right.
/// <para>
/// It is also high precision by construction: every legitimate change here has a proper
/// mechanism - the Intent MCP server, or a skill that drives it - so a denial can always
/// say where to go instead of merely saying no. An earlier version of this guard blocked
/// generated output instead, and in real use produced false positive after false positive:
/// authoring a scaffolded template, writing release notes and correcting a csproj package
/// version are all intended workflows, and all were denied.
/// </para>
/// </remarks>
public static class IntentMetadataGuard
{
    /// <summary>Records what is installed; a bad edit corrupts the application's module state.</summary>
    private const string ModulesConfig = "modules.config";

    /// <summary>
    /// Matched as suffixes rather than exact names, because each is prefixed with the
    /// owning application's name - "MyApp.application.config", and so on.
    /// </summary>
    private static readonly string[] ProtectedSuffixes =
    {
        ".application.config",
        ".application.managed-files.xml",
        ".application.output.config.xml",
        ".application.deviations.log.xml",
    };

    /// <summary>Any file anywhere beneath one of these is designer-owned model content.</summary>
    private static readonly string[] ProtectedDirectories =
    {
        "Intent.Metadata",
        ".intent",
    };

    /// <summary>
    /// The reason this path must not be hand-edited, or null when it is not Intent metadata.
    /// </summary>
    public static string? DescribeViolation(string path)
    {
        var fileName = Path.GetFileName(path);

        if (string.Equals(fileName, ModulesConfig, StringComparison.OrdinalIgnoreCase))
        {
            return "modules.config records what is installed and is never hand-edited. Install or "
                 + "update the module through Intent Architect instead - the Intent MCP server's "
                 + "install_or_update_modules, or the equivalent action in the UI.";
        }

        foreach (var suffix in ProtectedSuffixes)
        {
            if (fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return $"'{fileName}' is Intent Architect application metadata, owned by the designers "
                     + "and the Software Factory. Hand-editing it corrupts state that no regeneration "
                     + "puts right. Change it through the Intent MCP server, or a skill that drives it.";
            }
        }

        var segments = path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        foreach (var directory in ProtectedDirectories)
        {
            if (Array.Exists(segments, segment => string.Equals(segment, directory, StringComparison.OrdinalIgnoreCase)))
            {
                return $"This path is inside '{directory}', which holds Intent Architect's designer "
                     + "model. It is never edited directly. Use the Intent MCP server - "
                     + "run_designer_script to change the model - or a skill that drives it.";
            }
        }

        return null;
    }
}