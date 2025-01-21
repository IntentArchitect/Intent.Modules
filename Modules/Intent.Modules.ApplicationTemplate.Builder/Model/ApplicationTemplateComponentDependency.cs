#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateComponentDependency
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }
}