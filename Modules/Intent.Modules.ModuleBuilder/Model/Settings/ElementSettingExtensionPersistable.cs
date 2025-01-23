using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ElementSettingExtensionPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlArray("implements")]
        [XmlArrayItem("trait", typeof(ImplementedTraitPersistable))]
        public List<ImplementedTraitPersistable> Implements { get; set; } = [];
        public bool ShouldSerializeImplements() => Implements.Count != 0;

        [XmlElement("displayFunctionOverride")]
        public string DisplayFunctionOverride { get; set; }

        [XmlElement("validateFunctionOverride")]
        public string ValidateFunctionOverride { get; set; } // TODO: Rename. It doesn't override, it adds an additional validation

        [XmlArray("contextMenuOptions")]
        [XmlArrayItem("createElement", typeof(ElementCreationOption))]
        [XmlArrayItem("createAssociation", typeof(AssociationCreationOption))]
        [XmlArrayItem("createStereotype", typeof(StereotypeCreationOption))]
        [XmlArrayItem("runScript", typeof(RunScriptOption))]
        [XmlArrayItem("openMapping", typeof(MappingOption))]
        public required List<ContextMenuOption> ContextMenuOptions { get; set; } = new();

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOptionOld> CreationOptions { get; set; }

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
        public List<MacroPersistable> Macros { get; set; } = new List<MacroPersistable>();

        [XmlArray("childElementExtensions")]
        [XmlArrayItem("childElementExtension")]
        public ElementSettingExtensionPersistable[] ChildElementExtensions { get; set; } = [];
        public bool ShouldSerializeChildElementExtensions() => ChildElementExtensions.Any();
    }
}