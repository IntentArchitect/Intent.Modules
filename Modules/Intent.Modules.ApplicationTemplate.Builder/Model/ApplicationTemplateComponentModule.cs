#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateComponentModule
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("version")]
    public string Version { get; set; }

    [XmlAttribute("enableFactoryExtensions")]
    public bool EnableFactoryExtensions { get; set; } = true;
    public bool ShouldSerializeEnableFactoryExtensions() => !EnableFactoryExtensions;

    [XmlAttribute("installApplicationSettings")]
    public bool InstallApplicationSettings { get; set; } = true;
    public bool ShouldSerializeInstallApplicationSettings() => !InstallApplicationSettings;

    [XmlAttribute("installDesignerMetadata")]
    public bool InstallDesignerMetadata { get; set; } = true;
    public bool ShouldSerializeInstallDesignerMetadata() => !InstallDesignerMetadata;

    [XmlAttribute("installDesigners")]
    public bool InstallDesigners { get; set; } = true;
    public bool ShouldSerializeInstallDesigners() => !InstallDesigners;

    [XmlAttribute("installTemplateOutputs")]
    public bool InstallTemplateOutputs { get; set; } = true;
    public bool ShouldSerializeInstallTemplateOutputs() => !InstallTemplateOutputs;

    [XmlElement("includedByDefault")]
    public bool? IsIncludedByDefault { get; set; }
    public bool ShouldSerializeIsIncludedByDefault() => IsIncludedByDefault.HasValue;

    [XmlElement("required")]
    public bool? IsRequired { get; set; }
    public bool ShouldSerializeIsRequired() => IsRequired.HasValue;
}