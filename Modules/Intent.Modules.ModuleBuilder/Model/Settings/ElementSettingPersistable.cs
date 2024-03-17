using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ElementSettingPersistable
    {
        private string _specializationType;
        private List<TypeOrderPersistable> _typeOrder;

        [XmlAttribute("type")]
        public string SpecializationType
        {
            get => _specializationType ?? throw new Exception("SpecializationType not specified");
            set => _specializationType = value;
        }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("iconFunction")]
        public string IconFunction { get; set; }

        [XmlElement("expandedIcon")]
        public IconModelPersistable ExpandedIcon { get; set; }

        [XmlElement("saveAsOwnFile")]
        public bool SaveAsOwnFile { get; set; } = true;

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

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

        [XmlElement("allowRename")]
        public bool? AllowRename { get; set; } = true;
        public bool ShouldSerializeAllowRename() => AllowRename.HasValue;

        [XmlElement("allowAbstract")]
        public bool? AllowAbstract { get; set; }
        public bool ShouldSerializeAllowAbstract() => AllowAbstract.HasValue;

        [XmlElement("allowStatic")]
        public bool? AllowStatic { get; set; }
        public bool ShouldSerializeAllowStatic() => AllowStatic.HasValue;

        [XmlElement("allowGenericTypes")]
        public bool? AllowGenericTypes { get; set; }
        public bool ShouldSerializeAllowGenericTypes() => AllowGenericTypes.HasValue;

        [XmlElement("allowMapping")]
        public bool? AllowMapping { get; set; }
        public bool ShouldSerializeAllowMapping() => AllowMapping.HasValue;

        [XmlElement("allowSorting")]
        public bool? AllowSorting { get; set; }
        public bool ShouldSerializeAllowSorting() => AllowSorting.HasValue;

        [XmlElement("sortChildren")]
        public SortChildrenOptions? SortChildren { get; set; }

        [XmlElement("allowFindInView")]
        public bool? AllowFindInView { get; set; }

        [XmlElement("allowTypeReference")]
        public bool? AllowTypeReference { get; set; }

        [XmlElement("allowConvertToType")]
        public bool? AllowConvertToType { get; set; }

        [XmlElement("allowSetValue")]
        public bool? AllowSetValue { get; set; }

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

        [XmlArray("mappingSettings")]
        [XmlArrayItem("mappingSetting")]
        public List<MappingSettingPersistable> MappingSettings { get; set; } = new List<MappingSettingPersistable>();

        [XmlElement("diagramSettings")]
        public DiagramSettings DiagramSettings { get; set; }

        [XmlElement("visualSettings")]
        public ElementVisualSettingsPersistable VisualSettings { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<ElementMacroPersistable> Macros { get; set; } = new List<ElementMacroPersistable>();

        [XmlArray("childElementSettings")]
        [XmlArrayItem("childElementSetting")]
        public ElementSettingPersistable[] ChildElementSettings { get; set; } = Array.Empty<ElementSettingPersistable>();

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
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
    }

    // TODO: GCB - Rename to MacroPersistable (since used by associations too), but is breaking change with Module Builder module.
    public class ElementMacroPersistable
    {
        [XmlAttribute("trigger")]
        public string Trigger { get; set; }

        [XmlElement("script")]
        public string Script { get; set; }

        [XmlIgnore]
        public string Source { get; set; }
    }
}