// ReSharper disable once CheckNamespace
namespace Intent.Modules.Common.Templates;

/// <summary>
/// For templates which support migrations.
/// </summary>
public interface ISupportsMigrations
{
    /// <summary>
    /// The metadata of the template.
    /// </summary>
    TemplateMetadata TemplateMetadata { get; }

    /// <summary>
    /// Any migrations for the template.
    /// </summary>
    ITemplateMigration[] Migrations { get; }
}