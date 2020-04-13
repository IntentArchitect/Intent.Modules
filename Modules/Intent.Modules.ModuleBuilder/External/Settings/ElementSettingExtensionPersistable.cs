using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ElementSettingExtensionPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder { get; set; }
    }
}