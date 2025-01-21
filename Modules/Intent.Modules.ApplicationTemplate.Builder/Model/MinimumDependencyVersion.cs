#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class MinimumDependencyVersion
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("version")]
    public string Version { get; set; }
}