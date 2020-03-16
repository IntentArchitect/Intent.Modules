using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    [XmlRoot("stereotypeDefinition")]
    public class StereotypeDefinitionPersistable
    {
        public const string FileExtension = "xml";

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("displayIcon")]
        public bool DisplayIcon { get; set; }

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

        [XmlArray("targetElements")]
        [XmlArrayItem("element")]
        public List<string> TargetElements { get; set; }

        [XmlElement("autoAdd")]
        public bool AutoAdd { get; set; }

        [XmlElement("parentFolderId")]
        public string ParentFolderId { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<StereotypePropertyDefinitionPersistable> Properties { get; set; }

        public StereotypeDefinitionPersistable()
        {
            TargetElements = new List<string>();
            Properties = new List<StereotypePropertyDefinitionPersistable>();
        }

        public override string ToString()
        {
            return $"StereotypeDefinition: {Name}";
        }
    }
}