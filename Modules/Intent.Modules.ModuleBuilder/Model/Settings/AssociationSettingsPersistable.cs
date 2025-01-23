using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class AssociationSettingsPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("sourceEnd")]
        public AssociationEndSettingsPersistable SourceEnd { get; set; }

        [XmlElement("targetEnd")]
        public AssociationEndSettingsPersistable TargetEnd { get; set; }

        [XmlElement("visualSettings")]
        public AssociationVisualSettingsPersistable VisualSettings { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<MacroPersistable> Macros { get; set; } = new List<MacroPersistable>();

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
        }

        public void OnLoad(DesignerSettingsPersistable designerSettings)
        {
            foreach (var macro in Macros)
            {
                macro.Source = $"{designerSettings.Name} ({designerSettings.Source})";
            }
        }
    }

    public class AssociationEndSettingsPersistable
    {
        private List<TypeOrderPersistable> _typeOrder;

        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlArray("implements")]
        [XmlArrayItem("trait", typeof(ImplementedTraitPersistable))]
        public List<ImplementedTraitPersistable> Implements { get; set; }
        public bool ShouldSerializeImplements() => Implements?.Any() == true;

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

        [XmlElement("nameAccessibilityMode")]
        public FieldAccessibilityMode NameAccessibilityMode { get; set; }

        [XmlElement("defaultNameFunction")]
        public string DefaultNameFunction { get; set; }

        [XmlElement("nameMustBeUnique")]
        public bool? NameMustBeUnique { get; set; }
        public bool ShouldSerializeNameMustBeUnique() => NameMustBeUnique.HasValue;

        private string _validateFunction;
        [XmlElement("validateFunction")]
        public string ValidateFunction
        {
            get => _validateFunction;
            set
            {
                _validateFunction = value;
                if (ValidateFunctions.All(x => !x.Equals(value)))
                {
                    ValidateFunctions.Add(value);
                }
            }
        }
        public bool ShouldSerializeValidateFunction() => ValidateFunction != null;

        [XmlIgnore]
        public List<string> ValidateFunctions { get; } = new List<string>();

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("iconFunction")]
        public string IconFunction { get; set; }

        [XmlElement("allowSetValue")]
        public bool? AllowSetValue { get; set; }
        public bool ShouldSerializeAllowSetValue() => AllowSetValue.HasValue && AllowSetValue.Value;

        [XmlElement("allowSorting")]
        public bool? AllowSorting { get; set; }

        [XmlElement("sortChildren")]
        public SortChildrenOptions? SortChildren { get; set; }

        [XmlElement("typeReferenceSetting")]
        public TypeReferenceSettingPersistable TypeReferenceSetting { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type", typeof(TypeOrderPersistable))]
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

        [XmlArray("contextMenuOptions")]
        [XmlArrayItem("createElement", typeof(ElementCreationOption))]
        [XmlArrayItem("createAssociation", typeof(AssociationCreationOption))]
        [XmlArrayItem("createStereotype", typeof(StereotypeCreationOption))]
        [XmlArrayItem("runScript", typeof(RunScriptOption))]
        [XmlArrayItem("openMapping", typeof(MappingOption))]
        public required List<ContextMenuOption> ContextMenuOptions { get; set; } = new();

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOptionOld> CreationOptions { get; set; } = new();

        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; } = new();

        [XmlArray("mappingOptions")]
        [XmlArrayItem("option")]
        public List<MappingOption> MappingOptions { get; set; } = new();

        public void AdjustFromExtension(AssociationEndSettingExtensionPersistable extension)
        {
            if (!string.IsNullOrWhiteSpace(extension.ValidateFunction))
            {
                ValidateFunctions.Add(extension.ValidateFunction);
            }
            TypeReferenceSetting.AdjustFromExtension(extension.TypeReferenceExtension);

            foreach (var creationOption in extension.CreationOptions)
            {
                if (CreationOptions.Any(x => x.SpecializationTypeId == creationOption.SpecializationTypeId))
                {
                    throw new Exception($"Creation Option for [{creationOption.SpecializationType}] already exists for Element Setting with SpecializationType [{SpecializationType}].");
                }

                if (!int.TryParse(creationOption.Order, out var order))
                {
                    order = CreationOptions.Count;
                }
                CreationOptions.Insert(Math.Max(Math.Min(order, CreationOptions.Count), 0), creationOption);
            }

            foreach (var scriptOption in extension.ScriptOptions)
            {
                if (!int.TryParse(scriptOption.Order, out var order))
                {
                    order = ScriptOptions.Count;
                }
                ScriptOptions.Insert(Math.Max(Math.Min(order, ScriptOptions.Count), 0), scriptOption);
            }

            foreach (var mappingOption in extension.MappingOptions)
            {
                if (!int.TryParse(mappingOption.Order, out var order))
                {
                    order = ScriptOptions.Count;
                }
                MappingOptions.Insert(Math.Max(Math.Min(order, ScriptOptions.Count), 0), mappingOption);
            }

            foreach (var type in extension.TypeOrder)
            {
                if (TypeOrder.Any(x => x.Equals(type)))
                {
                    continue;
                }

                AddType(type);
            }
        }

        public void AddType(TypeOrderPersistable typeOrder)
        {
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

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
        }
    }

    public enum FieldAccessibilityMode
    {
        [XmlEnum("unknown")]
        Unknown = 0,
        [XmlEnum("optional")]
        Optional = 1,
        [XmlEnum("required")]
        Required = 2,
        [XmlEnum("disabled")]
        Disabled = 3,
        [XmlEnum("hidden")]
        Hidden = 4,
    }
}