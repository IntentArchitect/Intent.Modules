#nullable enable
using System;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

/// <summary>
/// Specialization of <see cref="TemplateFileConfig"/> for setting
/// metadata specific to C# templates.
/// </summary>
public class RazorFileConfig : CSharpFileConfig, IRazorFileConfig
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
        string? fileName = null,
        string fileExtension = "razor",
        string? dependsUpon = null)
        : base(
            codeGenType: Templates.CodeGenType.RazorMerger,
            className: className,
            @namespace: @namespace,
            relativeLocation: relativeLocation,
            overwriteBehaviour: overwriteBehaviour,
            fileName: fileName,
            fileExtension: fileExtension,
            dependsUpon: dependsUpon)
    {
        RazorMergerConfiguration = new RazorMergerConfiguration(this);
    }

    /// <summary>
    /// Provides an instance <see cref="Templates.RazorMergerConfiguration"/> which is wrapping this instance.
    /// </summary>
    public RazorMergerConfiguration RazorMergerConfiguration { get; }

    /// <summary>
    /// Allows configuring Razor Merger settings in a "fluent manner".
    /// </summary>
    /// <param name="configure">A delegate for configuring the <see cref="Templates.RazorMergerConfiguration"/>.</param>
    /// <returns>This same instance of <see cref="RazorFileConfig"/>.</returns>
    public RazorFileConfig ConfigureRazorMerger(Action<RazorMergerConfiguration> configure)
    {
        configure(RazorMergerConfiguration);
        return this;
    }

    /// <summary>
    /// Allows configuring Roslyn Weaver settings in a "fluent manner".
    /// </summary>
    /// <param name="configure">A delegate for configuring the <see cref="RoslynWeaverConfiguration"/>.</param>
    /// <returns>This same instance of <see cref="RazorFileConfig"/>.</returns>
    public new RazorFileConfig ConfigureRoslynWeaver(Action<RoslynWeaverConfiguration> configure)
    {
        configure(RoslynWeaverConfiguration);
        return this;
    }
}