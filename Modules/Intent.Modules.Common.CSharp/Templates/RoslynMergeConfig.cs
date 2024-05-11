using System;

// ReSharper disable once CheckNamespace
namespace Intent.Modules.Common.Templates;

/// <summary>
/// Use <see cref="ISupportsMigrations"/> instead.
/// </summary>
[Obsolete("See XML doc comments")]
public class RoslynMergeConfig
{
    /// <summary>
    /// Creates a new instance of <see cref="RoslynMergeConfig"/>.
    /// </summary>
    public RoslynMergeConfig(TemplateMetadata templateMetadata, params ITemplateMigration[] migrations)
    {
        TemplateMetadata = templateMetadata;
        Migrations = migrations;
    }

    /// <summary>
    /// The metadata of the template.
    /// </summary>
    public TemplateMetadata TemplateMetadata { get; }

    /// <summary>
    /// Any migrations for the template.
    /// </summary>
    public ITemplateMigration[] Migrations { get; }
}