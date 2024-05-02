// ReSharper disable once CheckNamespace
namespace Intent.Modules.Common.Templates;

/// <summary>
/// Metadata of a template stored on a template's output.
/// </summary>
/// <remarks>
/// Used for <see cref="ITemplateMigration"/>s.
/// </remarks>
public class TemplateMetadata
{
    /// <summary>
    /// Creates a new instance of <see cref="TemplateMetadata"/>.
    /// </summary>
    public TemplateMetadata(string templateId, TemplateVersion version)
    {
        TemplateId = templateId;
        Version = version;
    }

    /// <summary>
    /// The ID of the template.
    /// </summary>
    public string TemplateId { get; }

    /// <summary>
    /// The migration version of the template.
    /// </summary>
    public TemplateVersion Version { get; }
}