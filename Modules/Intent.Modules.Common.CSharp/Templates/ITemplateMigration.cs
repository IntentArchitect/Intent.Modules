// ReSharper disable once CheckNamespace
namespace Intent.Modules.Common.Templates;

/// <summary>
/// A migration for updating an existing file to align it with a template's current generated content.
/// </summary>
public interface ITemplateMigration
{
    /// <summary>
    /// Criteria for whether the migration should be executed.
    /// </summary>
    TemplateMigrationCriteria Criteria { get; }

    /// <summary>
    /// Performs the migration.
    /// </summary>
    /// <param name="currentText">The content of the existing file.</param>
    /// <returns>The migrated content.</returns>
    string Execute(string currentText);
}