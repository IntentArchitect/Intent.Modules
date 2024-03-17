using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class MappableElementSettingIdentifierPersistable
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }
}