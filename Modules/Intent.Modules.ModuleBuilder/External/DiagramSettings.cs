using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class DiagramSettings {

        [XmlElement("addNewElementsTo")]
        public DiagramAddNewElementsTo AddNewElementsTo { get; set; }

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public ElementCreationOption[] CreationOptions { get; set; }

        [XmlArray("classVisualSettings")]
        [XmlArrayItem("classVisualSetting")]
        public ClassVisualSettings[] ClassVisualSettings { get; set; }

        [XmlArray("associationVisualSettings")]
        [XmlArrayItem("associationVisualSetting")]
        public AssociationVisualSettings[] AssociationVisualSettings { get; set; }
    }

    public enum DiagramAddNewElementsTo
    {
        [XmlEnum(Name = "Package")]
        Package = 0,
        [XmlEnum(Name = "Self")]
        Self = 1,
    }
}