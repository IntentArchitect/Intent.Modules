#nullable enable
using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

/// <summary>
/// A wrapper for file custom metadata for reading and setting Razor Merging related configuration.
/// </summary>
public class RazorMergerConfiguration
{
    private readonly IDictionary<string, string?> _fileCustomMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="RazorMergerConfiguration"/>.
    /// </summary>
    public RazorMergerConfiguration(ITemplateFileConfig config)
    {
        _fileCustomMetadata = config.CustomMetadata;
    }

    /// <summary>
    /// Creates a new instance of <see cref="RazorMergerConfiguration"/>.
    /// </summary>
    public RazorMergerConfiguration(IFileMetadata fileMetadata)
    {
        _fileCustomMetadata = fileMetadata.CustomMetadata;
    }

    /// <summary>
    /// The default <see cref="Mode"/> for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentManaged(Mode.<see cref="DefaultMode">&lt;DefaultMode&gt;</see>)]</c> at the top of the file.
    /// </summary>
    public Mode? DefaultMode
    {
        get => GetMode("RazorMergerDefaultMode");
        set => SetMode("RazorMergerDefaultMode", value);
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="DefaultMode"/>.
    /// </summary>
    public RazorMergerConfiguration WithDefaultMode(Mode mode)
    {
        DefaultMode = mode;
        return this;
    }

    private Mode? GetMode(string key)
    {
        if (!_fileCustomMetadata.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.Parse<Mode>(value);
    }

    private void SetMode(string key, Mode? mode)
    {
        _fileCustomMetadata[key] = mode?.ToString();
    }
}