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

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlArray("implements")]
        [XmlArrayItem("trait", typeof(ImplementedTraitPersistable))]
        public List<ImplementedTraitPersistable> Implements { get; set; }
        public bool ShouldSerializeImplements() => Implements?.Any() == true;

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

        [XmlElement("nameMustBeUnique")]
        public bool? NameMustBeUnique { get; set; }
        public bool ShouldSerializeNameMustBeUnique() => NameMustBeUnique.HasValue;

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
        //public bool ShouldSerializeAllowSetValue() => AllowSetValue.HasValue && AllowSetValue.Value;

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

        public bool ShouldSerializeTypeOrder() => TypeOrder.Count > 0;

        //[XmlArray("acceptedChildren")]
        //[XmlArrayItem("accepts", typeof(AcceptedChildSettingPersistable))]
        //public List<AcceptedChildSettingPersistable> AcceptedChildren { get; set; }
        //public bool ShouldSerializeAcceptedChildren() => AcceptedChildren != null;


        [XmlArray("contextMenuOptions")]
        [XmlArrayItem("createElement", typeof(ElementCreationOption))]
        [XmlArrayItem("createAssociation", typeof(AssociationCreationOption))]
        [XmlArrayItem("createStereotype", typeof(StereotypeCreationOption))]
        [XmlArrayItem("runScript", typeof(RunScriptOption))]
        [XmlArrayItem("openMapping", typeof(MappingOption))]
        public required List<ContextMenuOption> ContextMenuOptions { get; set; } = new();

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOptionOld> CreationOptions { get; set; } = [];
        public bool ShouldSerializeCreationOptions() => CreationOptions.Any();


        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; } = [];
        public bool ShouldSerializeScriptOptions() => ScriptOptions.Any();

        [XmlArray("mappingOptions")]
        [XmlArrayItem("option")]
        public List<MappingOption> MappingOptions { get; set; } = [];
        public bool ShouldSerializeMappingOptions() => MappingOptions.Any();

        [XmlArray("mappingSettings")]
        [XmlArrayItem("mappingSetting")]
        public List<MappingSettingPersistable> MappingSettings { get; set; } = new List<MappingSettingPersistable>();

        [XmlElement("diagramSettings")]
        public DiagramSettings DiagramSettings { get; set; }

        [XmlElement("visualSettings")]
        public ElementVisualSettingsPersistable VisualSettings { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<MacroPersistable> Macros { get; set; } = new List<MacroPersistable>();

        [XmlArray("childElementSettings")]
        [XmlArrayItem("childElementSetting")]
        public ElementSettingPersistable[] ChildElementSettings { get; set; } = [];
        public bool ShouldSerializeChildElementSettings() => ChildElementSettings.Any();

        [XmlArray("childElementExtensions")]
        [XmlArrayItem("childElementExtension")]
        public ElementSettingExtensionPersistable[] ChildElementExtensions { get; set; } = [];
        public bool ShouldSerializeChildElementExtensions() => ChildElementExtensions.Any();

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
            _typeOrder = _typeOrder.Select((x, index) =>
            {
                x.Order ??= index;
                return x;
            }).OrderBy(x => x.Order).ThenBy(x => x.Type).ToList();
        }
    }

    public class ImplementedTraitPersistable
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    public class MacroPersistable
    {
        [XmlAttribute("trigger")]
        public string Trigger { get; set; }

        [XmlArray("dependencies")]
        [XmlArrayItem("dependency")]
        public List<TargetReferencePersistable> Dependencies { get; set; }
        public bool ShouldSerializeDependencies() => Dependencies?.Any() == true;

        [XmlElement("script")]
        public string Script { get; set; }

        [XmlIgnore]
        public string Source { get; set; }
    }

    //public class AcceptedChildSettingPersistable
    //{
    //    [XmlAttribute("order")]
    //    public int Order { get; set; }

    //    [XmlAttribute("acceptBy")]
    //    public AcceptsChildBy AcceptBy { get; set; }

    //    [XmlAttribute("specializationType")]
    //    public string SpecializationType { get; set; }
    //    public bool ShouldSerializeSpecializationType() => AcceptBy == AcceptsChildBy.Type;

    //    [XmlAttribute("specializationTypeId")]
    //    public string SpecializationTypeId { get; set; }
    //    public bool ShouldSerializeSpecializationTypeId() => AcceptBy == AcceptsChildBy.Type;

    //    [XmlElement("function")]
    //    public string AcceptsFunction { get; set; }
    //    public bool ShouldSerializeAcceptsFunction() => AcceptBy == AcceptsChildBy.Function;
    //}

    public enum AcceptsChildBy
    {
        [XmlEnum("type")]
        Type,
        [XmlEnum("function")]
        Function
    }
}