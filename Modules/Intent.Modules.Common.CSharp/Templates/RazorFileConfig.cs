using Intent.IArchitect.Agent.Persistence;
using System;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

/// <summary>
/// Specialization of <see cref="TemplateFileConfig"/> for setting
/// metadata specific to C# templates.
/// </summary>
public class RazorFileConfig : CSharpFileConfig
{
    /// <summary>
    /// Creates a new instance of <see cref="RazorFileConfig"/>
    /// </summary>
    /// <param name="className"></param>
    /// <param name="namespace"></param>
    /// <param name="relativeLocation"></param>
    /// <param name="overwriteBehaviour"></param>
    /// <param name="fileName"></param>
    /// <param name="fileExtension"></param>
    /// <param name="dependsUpon"></param>
    public RazorFileConfig(
        string className,
        string @namespace,
        string relativeLocation = "",
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
        string fileName = null,
        string fileExtension = "razor",
        string dependsUpon = null)
        : base(
            className,
            @namespace,
            relativeLocation,
            overwriteBehaviour,
            fileName,
            fileExtension,
            dependsUpon)
    {
    }

    /// <summary>
    /// Provides an instance <see cref="RazorMergerConfiguration"/> which is wrapping this instance.
    /// </summary>
    public RazorMergerConfiguration RazorMergerConfiguration { get; }

    /// <summary>
    /// Allows configuring Roslyn Weaver settings in a "fluent manner".
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public new RazorFileConfig ConfigureRazorMerger(Action<RoslynWeaverConfiguration> configure)
    {
        configure(RoslynWeaverConfiguration);
        return this;
    }

    /// <summary>
    /// Allows configuring Roslyn Weaver settings in a "fluent manner".
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public new RazorFileConfig ConfigureRoslynWeaver(Action<RoslynWeaverConfiguration> configure)
    {
        configure(RoslynWeaverConfiguration);
        return this;
    }
}

/// <summary>
/// A wrapper for <see cref="ITemplateFileConfig"/> for reading and setting Razor Merging related configuration.
/// </summary>
public class RazorMergerConfiguration(ITemplateFileConfig config)
{
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
        if (!config.CustomMetadata.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.Parse<Mode>(value);
    }

    private void SetMode(string key, Mode? mode)
    {
        config.CustomMetadata[key] = mode?.ToString();
    }
}