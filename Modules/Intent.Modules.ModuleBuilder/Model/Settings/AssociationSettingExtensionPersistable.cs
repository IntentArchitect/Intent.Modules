using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class AssociationSettingExtensionPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("sourceEndExtension")]
        public AssociationEndSettingExtensionPersistable SourceEndExtension { get; set; }

        [XmlElement("targetEndExtension")]
        public AssociationEndSettingExtensionPersistable TargetEndExtension { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<ElementMacroPersistable> Macros { get; set; } = new List<ElementMacroPersistable>();

        // TODO: GCB - Visual Extensions
        //[XmlElement("visualSettingExtension")]
        //public AssociationVisualSettingsPersistable VisualExtension { get; set; }

        public override string ToString()
        {
            return $"{Name} : {SpecializationType}";
        }

        public void OnLoad(DesignerSettingsPersistable designerSettings)
        {
            foreach (var macro in Macros)
            {
                macro.Source = $"{designerSettings.Name} ({designerSettings.Source})";
            }
        }
    }
}