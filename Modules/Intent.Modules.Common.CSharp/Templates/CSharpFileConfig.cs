using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

/// <summary>
/// Specialization of <see cref="TemplateFileConfig"/> for setting
/// metadata specific to C# templates.
/// </summary>
public class CSharpFileConfig : TemplateFileConfig
{
    /// <summary>
    /// Sets the C# file configuration.
    /// </summary>
    public CSharpFileConfig(
        string className,
        string @namespace,
        string relativeLocation = "",
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
        string fileName = null,
        string fileExtension = "cs",
        string dependsUpon = null)
        : this(
            codeGenType: Templates.CodeGenType.RoslynWeaver,
            className: className,
            @namespace: @namespace,
            relativeLocation: relativeLocation,
            overwriteBehaviour: overwriteBehaviour,
            fileName: fileName,
            fileExtension: fileExtension,
            dependsUpon: dependsUpon) { }

    /// <summary>
    /// Sets the C# file configuration.
    /// </summary>
    protected internal CSharpFileConfig(
        string codeGenType,
        string className,
        string @namespace,
        string relativeLocation,
        OverwriteBehaviour overwriteBehaviour,
        string fileName,
        string fileExtension,
        string dependsUpon)
        : base(fileName ?? className, fileExtension, relativeLocation, overwriteBehaviour, codeGenType)
    {
        RoslynWeaverConfiguration = new RoslynWeaverConfiguration(this);

        ClassName = className ?? throw new ArgumentNullException(nameof(className));

        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            Namespace = @namespace;
        }

        if (!string.IsNullOrWhiteSpace(dependsUpon))
        {
            CustomMetadata["Depends On"] = dependsUpon;
        }

        ApplyNamespaceFormatting = true;

        this.WithItemType("Compile");
    }

    /// <inheritdoc />
    public sealed override Dictionary<string, string> CustomMetadata => base.CustomMetadata;

    /// <summary>
    /// Whether to automatically apply formatting to C# files.
    /// </summary>
    public bool AutoFormat
    {
        get => CustomMetadata.TryGetValue(nameof(AutoFormat), out var value) && bool.TryParse(value, out var parsed) && parsed;
        set => CustomMetadata[nameof(AutoFormat)] = value.ToString();
    }

    /// <summary>
    /// Disables the automatic formatting of this C# by the Roslyn Weaving system.
    /// </summary>
    /// <returns></returns>
    public CSharpFileConfig DisableAutoFormat()
    {
        AutoFormat = false;
        return this;
    }

    /// <summary>
    /// The primary class name of this file.
    /// </summary>
    public string ClassName
    {
        get => CustomMetadata["ClassName"];
        set => CustomMetadata["ClassName"] = value;
    }

    /// <summary>
    /// The primary namespace for this file.
    /// </summary>
    public string Namespace
    {
        get => CustomMetadata["Namespace"];
        set => CustomMetadata["Namespace"] = value;
    }

    /// <summary>
    /// Whether to apply formatting (such as PascalCasing) to namespaces. 
    /// </summary>
    [FixFor_Version4] // See if we can get rid of this, not sure what it's even being used for.
    public bool ApplyNamespaceFormatting
    {
        get => bool.TryParse(CustomMetadata[nameof(ApplyNamespaceFormatting)], out var parsed) && parsed;
        set => CustomMetadata[nameof(ApplyNamespaceFormatting)] = value.ToString();
    }

    /// <summary>
    /// Use <see cref="RoslynWeaverConfiguration.WithTagMode"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpFileConfig IntentTagModeExplicit()
    {
        CustomMetadata["RoslynWeaverTagMode"] = "Explicit";
        return this;
    }

    /// <summary>
    /// Use <see cref="RoslynWeaverConfiguration.WithTagMode"/> instead.
    /// </summary>
    [Obsolete]
    public CSharpFileConfig IntentTagModeImplicit()
    {
        CustomMetadata["RoslynWeaverTagMode"] = "Implicit";
        return this;
    }

    /// <summary>
    /// Provides an instance <see cref="Templates.RoslynWeaverConfiguration"/> which is wrapping this instance.
    /// </summary>
    public RoslynWeaverConfiguration RoslynWeaverConfiguration { get; }

    /// <summary>
    /// Allows configuring Roslyn Weaver settings in a "fluent manner".
    /// </summary>
    /// <param name="configure">A delegate for configuring the <see cref="Templates.RoslynWeaverConfiguration"/>.</param>
    /// <returns>This same instance of <see cref="CSharpFileConfig"/>.</returns>
    public CSharpFileConfig ConfigureRoslynWeaver(Action<RoslynWeaverConfiguration> configure)
    {
        configure(RoslynWeaverConfiguration);
        return this;
    }
}