using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

/// <summary>
/// A wrapper for <see cref="ITemplateFileConfig"/> for reading and setting Roslyn Weaver related configuration.
/// </summary>
public class RoslynWeaverConfiguration
{
    private readonly IDictionary<string, string> _fileCustomMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="RoslynWeaverConfiguration"/>.
    /// </summary>
    public RoslynWeaverConfiguration(ITemplateFileConfig config)
    {
        _fileCustomMetadata = config.CustomMetadata;
    }

    /// <summary>
    /// Creates a new instance of <see cref="RoslynWeaverConfiguration"/>.
    /// </summary>
    public RoslynWeaverConfiguration(IFileMetadata fileMetadata)
    {
        _fileCustomMetadata = fileMetadata.CustomMetadata;
    }

    /// <summary>
    /// The default <see cref="Mode"/> for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentManaged(Mode.<see cref="DefaultMode">&lt;DefaultMode&gt;</see>)]</c> at the top of the file.
    /// </summary>
    public Mode? DefaultMode
    {
        get => GetMode("RoslynWeaverDefaultMode");
        set => SetMode("RoslynWeaverDefaultMode", value);
    }

    /// <summary>
    /// The default <see cref="Mode"/> attributes for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentManaged(Mode.&lt;<see cref="DefaultMode">DefaultMode</see>&gt;, Attributes = Mode.&lt;<see cref="DefaultAttributesMode">DefaultAttributesMode</see>&gt;)]</c> at the top of the file.
    /// </summary>
    public Mode? DefaultAttributesMode
    {
        get => GetMode("RoslynWeaverDefaultAttributesMode");
        set => SetMode("RoslynWeaverDefaultAttributesMode", value);
    }

    /// <summary>
    /// The default <see cref="Mode"/> attributes for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentManaged(Mode.&lt;<see cref="DefaultMode">DefaultMode</see>&gt;, Body = Mode.&lt;<see cref="DefaultBodyMode">DefaultBodyMode</see>&gt;)]</c> at the top of the file.
    /// </summary>
    public Mode? DefaultBodyMode
    {
        get => GetMode("RoslynWeaverDefaultBodyMode");
        set => SetMode("RoslynWeaverDefaultBodyMode", value);
    }

    /// <summary>
    /// The default <see cref="Mode"/> attributes for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentManaged(Mode.&lt;<see cref="DefaultMode">DefaultMode</see>&gt;, Comments = Mode.&lt;<see cref="DefaultCommentsMode">DefaultCommentsMode</see>&gt;)]</c> at the top of the file.
    /// </summary>
    public Mode? DefaultCommentsMode
    {
        get => GetMode("RoslynWeaverDefaultCommentsMode");
        set => SetMode("RoslynWeaverDefaultCommentsMode", value);
    }

    /// <summary>
    /// The default <see cref="Mode"/> attributes for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentManaged(Mode.&lt;<see cref="DefaultMode">DefaultMode</see>&gt;, Signature = Mode.&lt;<see cref="DefaultSignatureMode">DefaultSignatureMode</see>&gt;)]</c> at the top of the file.
    /// </summary>
    public Mode? DefaultSignatureMode
    {
        get => GetMode("RoslynWeaverDefaultSignatureMode");
        set => SetMode("RoslynWeaverDefaultSignatureMode", value);
    }

    /// <summary>
    /// The default <see cref="TagMode"/> for the file.
    /// Equivalent to having <c>[assembly: DefaultIntentTagMode(TagMode.<see cref="TagMode">&lt;TagMode&gt;</see>)]</c> at the top of the file.
    /// </summary>
    public TagMode? TagMode
    {
        get => GetTagMode("RoslynWeaverTagMode");
        set => SetTagMode("RoslynWeaverTagMode", value);
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="DefaultMode"/>.
    /// </summary>
    public RoslynWeaverConfiguration WithDefaultMode(Mode mode)
    {
        DefaultMode = mode;
        return this;
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="DefaultAttributesMode"/>.
    /// </summary>
    public RoslynWeaverConfiguration WithDefaultAttributesMode(Mode mode)
    {
        DefaultAttributesMode = mode;
        return this;
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="DefaultBodyMode"/>.
    /// </summary>
    public RoslynWeaverConfiguration WithDefaultBodyMode(Mode mode)
    {
        DefaultBodyMode = mode;
        return this;
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="DefaultCommentsMode"/>.
    /// </summary>
    public RoslynWeaverConfiguration WithDefaultCommentsMode(Mode mode)
    {
        DefaultCommentsMode = mode;
        return this;
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="DefaultSignatureMode"/>.
    /// </summary>
    public RoslynWeaverConfiguration WithDefaultSignatureMode(Mode mode)
    {
        DefaultSignatureMode = mode;
        return this;
    }

    /// <summary>
    /// Convenience "fluent style" method for setting <see cref="TagMode"/>.
    /// </summary>
    public RoslynWeaverConfiguration WithTagMode(TagMode mode)
    {
        TagMode = mode;
        return this;
    }

    private TagMode? GetTagMode(string key)
    {
        if (!_fileCustomMetadata.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.Parse<TagMode>(value);
    }

    private void SetTagMode(string key, TagMode? mode)
    {
        _fileCustomMetadata[key] = mode?.ToString();
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

    //private bool GetBool(string key)
    //{
    //    if (!_fileCustomMetadata.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
    //    {
    //        return false;
    //    }

    //    return bool.TryParse(value, out var parsedValue) && parsedValue;
    //}

    //private void SetBool(string key, bool value)
    //{
    //    _fileCustomMetadata[key] = value.ToString();
    //}
}