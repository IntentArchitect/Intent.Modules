using System;

// ReSharper disable once CheckNamespace
namespace Intent.Modules.Common.Templates;

/// <summary>
/// Marker interfaces should no longer used to determine if a file supports merging and instead
/// a template's custom metadata should have the appropriate key set in it.
/// </summary>
[Obsolete("See XML doc comments")]
public interface IRoslynMerge
{

    /// <inheritdoc cref="IRoslynMerge"/>
    [Obsolete("See XML doc comments")]
    RoslynMergeConfig ConfigureRoslynMerger();
}