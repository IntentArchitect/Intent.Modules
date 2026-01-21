using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class DiagramSettings
    {

        [XmlElement("addNewElementsTo")]
        public DiagramAddNewElementsTo AddNewElementsTo { get; set; }

        [XmlArray("contextMenuOptions")]
        [XmlArrayItem("createElement", typeof(ElementCreationOption))]
        [XmlArrayItem("createAssociation", typeof(AssociationCreationOption))]
        [XmlArrayItem("createStereotype", typeof(StereotypeCreationOption))]
        [XmlArrayItem("runScript", typeof(RunScriptOption))]
        [XmlArrayItem("openMapping", typeof(MappingOption))]
        public required List<ContextMenuOption> ContextMenuOptions { get; set; } = [];

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOptionOld> CreationOptions { get; set; } = [];
        public bool ShouldSerializeCreationOptions() => CreationOptions?.Count == 0;

        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; } = [];
        public bool ShouldSerializeScriptOptions() => ScriptOptions?.Count == 0;

        [XmlArray("elementVisualSettings")]
        [XmlArrayItem("visualSettings")]
        public List<ElementVisualSettingsPersistable> ElementVisualSettings { get; set; } = new List<ElementVisualSettingsPersistable>();

        [XmlArray("associationVisualSettings")]
        [XmlArrayItem("visualSettings")]
        public List<AssociationVisualSettingsPersistable> AssociationVisualSettings { get; set; } = new List<AssociationVisualSettingsPersistable>();
    }

    public enum DiagramAddNewElementsTo
    {
        [XmlEnum(Name = "Package")]
        Package = 0,
        [XmlEnum(Name = "Self")]
        Self = 1,
    }
}