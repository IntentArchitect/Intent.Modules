using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public enum SortChildrenOptions
    {
        [XmlEnum("manually")]
        Manually = 0,
        [XmlEnum("by-type-then-manually")]
        SortByTypeThenManually = 1,
        [XmlEnum("by-type-then-by-name")]
        SortByTypeAndName = 2,
        [XmlEnum("by-name")]
        SortByName = 3
    }

    public class PackageSettingsPersistable
    {
        private List<TypeOrderPersistable> _typeOrder;

        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("sortChildren")]
        public SortChildrenOptions SortChildren { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlArray("requiredPackages")]
        [XmlArrayItem("package")]
        public string[] RequiredPackages { get; set; } = new string[0];

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

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder
        {
            get => _typeOrder;
            set
            {
                if (value == null)
                {
                    _typeOrder = null;
                    return;
                }

                _typeOrder = value;
                foreach (var typeOrder in value.ToList().Where(x => x.Order.HasValue))
                {
                    _typeOrder.Remove(typeOrder);
                    _typeOrder.Insert(Math.Max(Math.Min(typeOrder.Order.Value, _typeOrder.Count), 0), typeOrder);
                }
                UpdateTypesOrdering();
            }
        }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<MacroPersistable> Macros { get; set; }

        [XmlArray("childElementExtensions")]
        [XmlArrayItem("childElementExtension")]
        public ElementSettingExtensionPersistable[] ChildElementExtensions { get; set; } = [];
        public bool ShouldSerializeChildElementExtensions() => ChildElementExtensions.Any();

        public void AddType(TypeOrderPersistable typeOrder)
        {
            // NOTE: there is a check before this is called - need to consolidate
            if (TypeOrder.Any(x => x.Type == typeOrder.Type))
            {
                throw new Exception($"{nameof(TypeOrder)} could not be added to {this.SpecializationType} as it already contains {typeOrder}");
            }

            TypeOrder.Add(typeOrder);
            UpdateTypesOrdering();
        }

        private void UpdateTypesOrdering()
        {
            _typeOrder = _typeOrder.Select((x, index) =>
            {
                x.Order ??= index;
                return x;
            }).OrderBy(x => x.Order).ThenBy(x => x.Type).ToList();
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