using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class AssociationSettingsPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("sourceEnd")]
        public AssociationEndSettingsPersistable SourceEnd { get; set; }

        [XmlElement("targetEnd")]
        public AssociationEndSettingsPersistable TargetEnd { get; set; }

        [XmlElement("visualSettings")]
        public AssociationVisualSettingsPersistable VisualSettings { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
        }
    }

    public class AssociationEndSettingsPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

        [XmlElement("typeReferenceSetting")]
        public TypeReferenceSettingPersistable TypeReferenceSetting { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
        }
    }
}