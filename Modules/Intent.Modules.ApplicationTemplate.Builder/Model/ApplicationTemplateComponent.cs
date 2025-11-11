#nullable disable
using Intent.IArchitect.Agent.Persistence.Model.ApplicationTemplate;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateComponent
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("included")]
    public required bool Included { get; set; }

    [XmlAttribute("required")]
    public required bool Required { get; set; }

    [XmlAttribute("isNew")]
    public required bool IsNew { get; set; }
    public bool ShouldSerializeIsNew() => IsNew;

    [XmlElement("description")]
    public required string Description { get; set; }

    [XmlElement("icon")]
    public required IconModelPersistable Icon { get; set; }

    [XmlArray("modules")]
    [XmlArrayItem("module")]
    public required List<ApplicationTemplateComponentModule> Modules { get; set; } = [];

    [XmlArray("dependencies")]
    [XmlArrayItem("dependency")]
    public required List<ApplicationTemplateComponentDependency> Dependencies { get; set; } = [];
    //public bool ShouldSerializeDependencies() => Dependencies.Count != 0;

    [XmlArray("incompatibilities")]
    [XmlArrayItem("incompatible")]
    public required List<ApplicationTemplateComponentDependency> Incompatibilities { get; set; } = [];
    public bool ShouldSerializeIncompatibilities() => Incompatibilities.Count != 0;

    [XmlElement("minimumLicense")]
    public required string MinimumLicenseRequired { get; set; }
    public bool ShouldSerializeMinimumLicenseRequired() => !string.IsNullOrWhiteSpace(MinimumLicenseRequired);

    [XmlElement("documentationUrl")]
    public required string DocumentationUrl { get; set; }
    public bool ShouldSerializeDocumentationUrl() => !string.IsNullOrWhiteSpace(DocumentationUrl);

    [XmlElement("tags")]
    public required string Tags { get; set; } = "";
    public bool ShouldSerializeTags() => !string.IsNullOrWhiteSpace(Tags);
}