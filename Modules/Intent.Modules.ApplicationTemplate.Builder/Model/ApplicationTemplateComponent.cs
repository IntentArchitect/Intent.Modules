#nullable disable
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateComponent
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("included")]
    public bool Included { get; set; }

    [XmlAttribute("required")]
    public bool Required { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("icon")]
    public IconModelPersistable Icon { get; set; }

    [XmlArray("modules")]
    [XmlArrayItem("module")]
    public List<ApplicationTemplateComponentModule> Modules { get; set; }

    [XmlArray("dependencies")]
    [XmlArrayItem("dependency")]
    public List<ApplicationTemplateComponentDependency> Dependencies { get; set; }
}