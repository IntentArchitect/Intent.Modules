using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class AdvancedMappingTypePersistable
{
    [XmlAttribute("type")]
    public string MappingType { get; set; }

    [XmlAttribute("typeId")]
    public string MappingTypeId { get; set; }

    [XmlElement("represents")]
    public ElementMappingRepresentation? Represents { get; set; }
    public bool ShouldSerializeRepresents() => Represents.HasValue;

    [XmlArray("sources")]
    [XmlArrayItem("source")]
    public List<MappableElementSettingIdentifierPersistable> SourceTypes { get; set; }

    [XmlElement("sourceFilterFunction")]
    public string SourceFilterFunction { get; set; }

    [XmlElement("sourceArrowFunction")]
    public string SourceArrowFunction { get; set; }

    [XmlArray("targets")]
    [XmlArrayItem("target")]
    public List<MappableElementSettingIdentifierPersistable> TargetTypes { get; set; }

    [XmlElement("targetFilterFunction")]
    public string TargetFilterFunction { get; set; }

    [XmlElement("targetArrowFunction")]
    public string TargetArrowFunction { get; set; }

    [XmlElement("lineColor")]
    public string LineColor { get; set; }

    [XmlElement("lineDashArray")]
    public string LineDashArray { get; set; }

    [XmlElement("validationFunction")]
    public string ValidationFunction { get; set; }

    [XmlElement("allowAutoMap")]
    public bool AllowAutoMap { get; set; }
}

