using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class MappingSettingsPersistable
    {
        [XmlElement("defaultModeler")]
        public string DefaultModeler { get; set; }

        [XmlArray("mappings")]
        [XmlArrayItem("mapping")]
        public List<ElementMappingSettingPersistable> MappedTypes { get; set; }
    }

    public class ElementMappingSettingPersistable
    {
        [XmlElement("criteria")]
        public ElementMappingCriteriaSettingPersistable Criteria { get; set; } = new ElementMappingCriteriaSettingPersistable();

        [XmlElement("mapTo")]
        public ElementMappingMapToSettingPersistable MapTo { get; set; } = new ElementMappingMapToSettingPersistable();

        [XmlArray("childMappings")]
        [XmlArrayItem("mapping")]
        public List<ElementMappingSettingPersistable> ChildMappingSettings { get; set; } = new List<ElementMappingSettingPersistable>();
    }

    public class ElementMappingCriteriaSettingPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("hasTypeReference")]
        public bool? HasTypeReference { get; set; }

        [XmlElement("isCollection")]
        public bool? IsCollection { get; set; }

        [XmlElement("hasChildren")]
        public bool? HasChildren { get; set; }
    }

    public class ElementMappingMapToSettingPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("typeReferenceCreation")]
        public ElementMappingTypeCreationSettingPersistable TypeReferenceCreation { get; set; }
    }

    public class ElementMappingTypeCreationSettingPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("nameFunction")]
        public string NameFunction { get; set; }
    }
}