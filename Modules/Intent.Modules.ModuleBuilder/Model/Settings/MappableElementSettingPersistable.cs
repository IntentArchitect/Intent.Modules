using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class MappableElementSettingPersistable
{
    private bool? _isTraversable;

    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("type")]
    public string SpecializationType { get; set; }

    [XmlAttribute("typeId")]
    public string SpecializationTypeId { get; set; }

    [XmlElement("filterFunction")]
    public string FilterFunction { get; set; }

    [XmlElement("isRequiredFunction")]
    public string IsRequiredFunction { get; set; }

    [XmlElement("allowMultipleMappings")]
    public bool? AllowMultipleMappings { get; set; }

    [XmlElement("isMappableFunction")]
    public string IsMappableFunction { get; set; }

    [XmlElement("isTraversable")]
    public bool IsTraversable
    {
        get => _isTraversable ?? TraversableTypes.Any();
        set => _isTraversable = value;
    }

    [XmlElement("getTraversableTypeFunction")]
    public string GetTraversableTypeFunction { get; set; }

    [XmlArray("traversableTypes")]
    [XmlArrayItem("mappingId")]
    public List<string> TraversableTypes { get; set; } = new ();

    [XmlElement("canBeModified")]
    public bool CanBeModified { get; set; }

    [XmlElement("createNameFunction")]
    public string CreateNameFunction { get; set; }

    [XmlElement("useChildSettingsFrom")]
    public string UseChildSettingsFrom { get; set; }

    [XmlElement("represents")]
    public string Represents { get; set; }

    [XmlArray("childSettings")]
    [XmlArrayItem("childSetting")]
    public List<MappableElementSettingPersistable> ChildSettings { get; set; }
}