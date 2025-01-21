#nullable disable
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateComponentGroup
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("selectionMode")]
    public ComponentGroupSelectionMode SelectionMode { get; set; } = ComponentGroupSelectionMode.AllowSelectMultiple;

    [XmlArray("components")]
    [XmlArrayItem("component")]
    public List<ApplicationTemplateComponent> Components { get; set; }
}