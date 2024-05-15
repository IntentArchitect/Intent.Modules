using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class AdvancedMappingSettingsPersistable
{
    [XmlAttribute("type")]
    public string MappingType { get; set; }

    [XmlAttribute("typeId")]
    public string MappingTypeId { get; set; }

    [XmlElement("title")]
    public string Title { get; set; }

    [XmlElement("isRequiredFunction")] 
    public string IsRequiredFunction { get; set; }

    [XmlElement("sourceRootElementFunction")]
    public string SourceRootElementFunction { get; set; }

    [XmlElement("targetRootElementFunction")]
    public string TargetRootElementFunction { get; set; }

    [XmlArray("sourceMappings")]
    [XmlArrayItem("importSettings", typeof(MappableElementsPackageImportPersistable))]
    [XmlArrayItem("reference", typeof(MappableElementSettingIdentifierPersistable))]
    [XmlArrayItem("mapping", typeof(MappableElementSettingPersistable))]
    public List<object> SourceMappableSettings { get; set; }

    [XmlIgnore]
    public List<MappableElementSettingPersistable> SourceSettings { get; set; }

    [XmlArray("targetMappings")]
    [XmlArrayItem("importSettings", typeof(MappableElementsPackageImportPersistable))]
    [XmlArrayItem("reference", typeof(MappableElementSettingIdentifierPersistable))]
    [XmlArrayItem("mapping", typeof(MappableElementSettingPersistable))]
    public List<object> TargetMappableSettings { get; set; }

    [XmlIgnore]
    public List<MappableElementSettingPersistable> TargetSettings { get; set; }

    [XmlArray("mappingTypes")]
    [XmlArrayItem("mappingType")]
    public List<AdvancedMappingTypePersistable> MappingTypes { get; set; }
}