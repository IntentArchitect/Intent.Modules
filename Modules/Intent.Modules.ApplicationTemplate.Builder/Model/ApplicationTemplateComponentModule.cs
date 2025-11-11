using System;
using System.Xml.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateComponentModule
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("version")]
    public required string Version { get; set; }

    /// <inheritdoc cref="MetadataOnly"/>
    /// <remarks>
    /// This cannot have <see cref="ObsoleteAttribute"/> applied to it as that implicitly adds XmlIgnore, see
    /// https://stackoverflow.com/questions/331013/obsolete-attribute-causes-property-to-be-ignored-by-xmlserialization.
    /// </remarks>
    [XmlAttribute("metadataOnly")]
    public string MetadataOnlyObsolete { get; set; }
    public bool ShouldSerializeMetadataOnlyObsolete() => bool.TryParse(MetadataOnlyObsolete, out var result) && result;

    /// <summary>
    /// Obsolete. Use any combination of the following granular installation settings instead:
    /// <list type="bullet">
    /// <item><see cref="EnableFactoryExtensions"/></item>
    /// <item><see cref="InstallApplicationSettings"/></item>
    /// <item><see cref="InstallDesignerMetadata"/></item>
    /// <item><see cref="InstallDesigners"/></item>
    /// <item><see cref="InstallTemplateOutputs"/></item>
    /// </list>
    /// </summary>
    [Obsolete]
    [XmlIgnore]
    public bool MetadataOnly
    {
        get => bool.TryParse(MetadataOnlyObsolete, out var result)
            ? result
            : IncludeAssetsHelper.IncludesNone(IncludeAssets);
        set => MetadataOnlyObsolete = value.ToString().ToLowerInvariant();
    }

    [XmlAttribute("includeAssets")]
    public required string? IncludeAssets { get; set; }
    public bool ShouldSerializeIncludeAssets() => !string.IsNullOrWhiteSpace(IncludeAssets);

    [XmlIgnore]
    public bool EnableFactoryExtensions => IncludeAssetsHelper.IncludesFactoryExtensions(IncludeAssets);

    [XmlIgnore]
    public bool InstallApplicationSettings => IncludeAssetsHelper.IncludesApplicationSettings(IncludeAssets);

    [XmlIgnore]
    public bool InstallDesignerMetadata => IncludeAssetsHelper.IncludesDesignerMetadata(IncludeAssets);

    [XmlIgnore]
    public bool InstallDesigners => IncludeAssetsHelper.IncludesDesigners(IncludeAssets);

    [XmlIgnore]
    public bool InstallTemplateOutputs => IncludeAssetsHelper.IncludesTemplateOutputs(IncludeAssets);

    [XmlElement("includedByDefault")]
    public required bool? IsIncludedByDefault { get; set; }
    public bool ShouldSerializeIsIncludedByDefault() => IsIncludedByDefault.HasValue;

    [XmlElement("required")]
    public required bool? IsRequired { get; set; }
    public bool ShouldSerializeIsRequired() => IsRequired.HasValue;

    [XmlElement("isNew")]
    public required bool? IsNew { get; set; }
    public bool ShouldSerializeIsNew() => IsNew.HasValue && IsNew.Value;

    internal void OnAfterDeserialization()
    {
        (IncludeAssets, MetadataOnlyObsolete) = IncludeAssetsHelper.OnAfterDeserialization(IncludeAssets, MetadataOnlyObsolete);
    }
}