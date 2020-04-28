using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class AssociationSettingsPersistable
    {
        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

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
        [XmlElement("typeReferenceSetting")]
        public TypeReferenceSettingPersistable TypeReferenceSetting { get; set; }
    }
}