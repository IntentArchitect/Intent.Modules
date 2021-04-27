using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ElementSettingExtensionPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder { get; set; }

        [XmlElement("mappingSettings")]
        public MappingSettingsPersistable MappingSettings { get; set; }

        [XmlElement("typeReferenceSetting")]
        public TypeReferenceExtensionSettingPersistable TypeReferenceExtensionSetting { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<ElementMacroPersistable> Macros { get; set; } = new List<ElementMacroPersistable>();
    }

    public class TypeReferenceExtensionSettingPersistable
    {
        [XmlElement("isRequired")]
        public bool IsRequired { get; set; }

        [XmlArray("targetTypes")]
        [XmlArrayItem("type")]
        public string[] TargetTypes { get; set; }

        [XmlElement("defaultTypeId")]
        public string DefaultTypeId { get; set; }

        [XmlElement("allowIsNullable")]
        public BooleanExtensionOptions AllowIsNullable { get; set; } = BooleanExtensionOptions.Inherit;

        [XmlElement("allowIsCollection")]
        public BooleanExtensionOptions AllowIsCollection { get; set; } = BooleanExtensionOptions.Inherit;
    }

    public enum BooleanExtensionOptions
    {
        [XmlEnum("inherit")]
        Inherit = 0,
        [XmlEnum("true")]
        True = 1,
        [XmlEnum("false")]
        False = 2
    }
}