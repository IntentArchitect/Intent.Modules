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
        public List<ElementMacroPersistable> Macros { get; set; } = new List<ElementMacroPersistable>();

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

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

        [XmlElement("nameAccessibilityMode")]
        public FieldAccessibilityMode NameAccessibilityMode { get; set; }

        [XmlElement("defaultNameFunction")]
        public string DefaultNameFunction { get; set; }

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
                foreach (var typeOrder in value.ToList().Where(x => !string.IsNullOrWhiteSpace(x.Order)))
                {
                    _typeOrder.Remove(typeOrder);
                    _typeOrder.Insert(Math.Max(Math.Min(int.Parse(typeOrder.Order), _typeOrder.Count), 0), typeOrder);
                }
                UpdateTypesOrdering();
            }
        }

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; } = new();

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
            _typeOrder = _typeOrder.Select((x, index) => new TypeOrderPersistable()
            {
                Type = x.Type,
                Order = !string.IsNullOrWhiteSpace(x.Order) ? x.Order : index.ToString()
            }).OrderBy(x => int.Parse(x.Order)).ThenBy(x => x.Type).ToList();
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