using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class SuggestionSettingPersistable
{
    [XmlAttribute("type")]
    public string SpecializationType { get; set; }

    [XmlAttribute("typeId")]
    public string SpecializationTypeId { get; set; }

    [XmlAttribute("locations")]
    public string Locations { get; set; }

    [XmlElement("name")]
    public string Name { get; set; }

    [XmlElement("filterFunction")]
    public string FilterFunction { get; set; }

    [XmlElement("displayFunction")]
    public string DisplayFunction { get; set; }

    [XmlArray("dependencies")]
    [XmlArrayItem("dependency")]
    public List<TargetReferencePersistable> Dependencies { get; set; }

    [XmlElement("script")]
    public string Script { get; set; }
}