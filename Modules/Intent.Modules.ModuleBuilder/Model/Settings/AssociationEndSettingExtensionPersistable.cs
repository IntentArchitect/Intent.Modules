using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class AssociationEndSettingExtensionPersistable
    {
        [XmlElement("validateFunction")]
        public string ValidateFunction { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder { get; set; } = new();

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; }

        [XmlArray("mappingOptions")]
        [XmlArrayItem("option")]
        public List<MappingOption> MappingOptions { get; set; }

        [XmlElement("typeReferenceExtension")]
        public TypeReferenceExtensionSettingPersistable TypeReferenceExtension { get; set; }

    }
}