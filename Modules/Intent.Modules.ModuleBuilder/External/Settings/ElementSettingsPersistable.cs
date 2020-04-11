using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ElementSettingsPersistable
    {
        private string _specializationType;
        private List<TypeOrderPersistable> _typeOrder;

        [XmlAttribute("type")]
        public string SpecializationType
        {
            get => _specializationType ?? SpecializationTypeOld;
            set => _specializationType = value;
        }

        [XmlElement("specializationType")]
        public string SpecializationTypeOld { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("expandedIcon")]
        public IconModelPersistable ExpandedIcon { get; set; }

        [XmlElement("saveAsOwnFile")]
        public bool SaveAsOwnFile { get; set; } = true;

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

        [XmlElement("allowRename")]
        public bool? AllowRename { get; set; } = true;

        [XmlElement("allowAbstract")]
        public bool? AllowAbstract { get; set; }

        [XmlElement("allowGenericTypes")]
        public bool? AllowGenericTypes { get; set; }

        [XmlElement("allowMapping")]
        public bool? AllowMapping { get; set; }

        [XmlElement("allowSorting")]
        public bool? AllowSorting { get; set; }

        [XmlElement("allowFindInView")]
        public bool? AllowFindInView { get; set; }

        [XmlElement("allowTypeReference")]
        public bool? AllowTypeReference { get; set; }

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
                    _typeOrder.Insert(Math.Min(int.Parse(typeOrder.Order), _typeOrder.Count), typeOrder);
                }
            }
        }

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlElement("mappingSettings")]
        public MappingSettingsPersistable MappingSettings { get; set; }

        [XmlElement("diagramSettings")]
        public DiagramSettings DiagramSettings { get; set; }

        [XmlArray("literalSettings")]
        [XmlArrayItem("literalSetting")]
        public ClassLiteralSettings[] LiteralSettings { get; set; }

        [XmlArray("attributeSettings")]
        [XmlArrayItem("attributeSetting")]
        public AttributeSettingsPersistable[] AttributeSettings { get; set; }

        [XmlArray("operationSettings")]
        [XmlArrayItem("operationSetting")]
        public OperationSettingsPersistable[] OperationSettings { get; set; }

        [XmlArray("childElementSettings")]
        [XmlArrayItem("childElementSetting")]
        public ElementSettingsPersistable[] ChildElementSettings { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
        }
    }
}