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

        [XmlElement("displayFunctionOverride")]
        public string DisplayFunctionOverride { get; set; }

        [XmlElement("validateFunctionOverride")]
        public string ValidateFunctionOverride { get; set; } // TODO: Rename. It doesn't override, it adds an additional validation

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; }

        [XmlArray("mappingOptions")]
        [XmlArrayItem("option")]
        public List<MappingOption> MappingOptions { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder { get; set; }

        [XmlArray("mappingSettings")]
        [XmlArrayItem("mappingSetting")]
        public List<MappingSettingPersistable> MappingSettings { get; set; } = new List<MappingSettingPersistable>();

        [XmlElement("typeReferenceSetting")]
        public TypeReferenceExtensionSettingPersistable TypeReferenceExtensionSetting { get; set; }

        [XmlElement("diagramSettings")]
        public DiagramSettings DiagramSettings { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<ElementMacroPersistable> Macros { get; set; } = new List<ElementMacroPersistable>();
    }
}