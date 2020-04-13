using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ModelerSettingsPersistable
    {
        [XmlElement("diagramSettings")]
        public DiagramSettings DiagramSettings { get; set; }

        [XmlElement("packageSettings")]
        public PackageSettingsPersistable PackageSettings { get; set; }

        [XmlArray("elementSettings")]
        [XmlArrayItem("elementSetting")]
        public List<ElementSettingPersistable> ElementSettings { get; set; }

        [XmlArray("associationSettings")]
        [XmlArrayItem("associationSetting")]
        public List<AssociationSettingsPersistable> AssociationSettings { get; set; }

        [XmlArray("elementExtensions")]
        [XmlArrayItem("elementExtension")]
        public List<ElementSettingExtensionPersistable> ElementExtensions { get; set; }

        [XmlElement("stereotypeSettings")]
        public StereotypeSettingsPersistable StereotypeSettings { get; set; }
    }

    public class StereotypeSettingsPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlArray("targetTypeOptions")]
        [XmlArrayItem("option")]
        public List<StereotypeTargetTypeOption> TargetTypeOptions { get; set; }
    }

    public class StereotypeTargetTypeOption
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlAttribute("displayText")]
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(DisplayText)} = '{DisplayText}'";
        }
    }

    public class TypeReferenceSettingPersistable
    {
        [XmlElement("isRequired")]
        public bool IsRequired { get; set; } = true;

        [XmlArray("targetTypes")]
        [XmlArrayItem("type")]
        public string[] TargetTypes { get; set; }

        [XmlElement("defaultTypeId")]
        public string DefaultTypeId { get; set; }

        [XmlElement("allowIsNavigable")]
        public bool AllowIsNavigable { get; set; } = true;

        [XmlElement("allowIsNullable")]
        public bool AllowIsNullable { get; set; } = true;

        [XmlElement("allowIsCollection")]
        public bool AllowIsCollection { get; set; } = true;

        [XmlElement("isNavigableDefault")]
        public bool IsNavigableDefault { get; set; } = true;

        [XmlElement("isNullableDefault")]
        public bool IsNullableDefault { get; set; }

        [XmlElement("isCollectionDefault")]
        public bool IsCollectionDefault { get; set; }
    }

    public class TypeOrderPersistable
    {
        private string _order;

        [XmlText]
        public string Type { get; set; }

        [XmlAttribute("order")]
        public string Order
        {
            get => _order;
            set => _order = int.TryParse(value, out var o) ? o.ToString() : null;
        }

        public override string ToString()
        {
            return $"{nameof(Type)} = '{Type}', " +
                   $"{nameof(Order)} = '{Order}'";
        }
    }

    public class AttributeSettingsPersistable
    {
        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("shortcut")]
        public string Shortcut { get; set; }

        [XmlElement("displayFunction")]
        public string DisplayFunction { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("allowRename")]
        public bool? AllowRename { get; set; } = true;

        [XmlElement("allowDuplicateNames")]
        public bool? AllowDuplicateNames { get; set; }

        [XmlElement("allowFindInView")]
        public bool? AllowFindInView { get; set; }

        [XmlElement("defaultTypeId")]
        public string DefaultTypeId { get; set; }

        [XmlArray("targetTypes")]
        [XmlArrayItem("type")]
        public string[] TargetTypes { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }

    public class ClassLiteralSettings
    {
        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("shortcut")]
        public string Shortcut { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("allowRename")]
        public bool? AllowRename { get; set; } = true;

        [XmlElement("allowDuplicateNames")]
        public bool? AllowDuplicateNames { get; set; }

        [XmlElement("allowFindInView")]
        public bool? AllowFindInView { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }

    public class OperationSettingsPersistable
    {
        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("shortcut")]
        public string Shortcut { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("allowRename")]
        public bool? AllowRename { get; set; } = true;

        [XmlElement("allowDuplicateNames")]
        public bool? AllowDuplicateNames { get; set; }

        [XmlElement("allowFindInView")]
        public bool? AllowFindInView { get; set; }

        [XmlElement("defaultTypeId")]
        public string DefaultTypeId { get; set; }

        [XmlArray("targetTypes")]
        [XmlArrayItem("type")]
        public string[] TargetTypes { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }

    public class PackageSettingsPersistable
    {
        private List<TypeOrderPersistable> _typeOrder;

        [XmlArray("requiredPackages")]
        [XmlArrayItem("package")]
        public string[] RequiredPackages { get; set; } = new string[0];

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

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
                foreach (var typeOrder in value.ToList().Where(x => !string.IsNullOrWhiteSpace(x.Order)))
                {
                    _typeOrder.Remove(typeOrder);
                    _typeOrder.Insert(Math.Min(int.Parse(typeOrder.Order), _typeOrder.Count), typeOrder);
                }
            }
        }
    }


    public class ElementCreationOption
    {
        [XmlAttribute("order")]
        public string Order { get; set; }

        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("shortcut")]
        public string Shortcut { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("allowMultiple")]
        public bool AllowMultiple { get; set; } = true;

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }

    public enum ElementType
    {
        Unknown = 0,
        Class = 1,
        Literal = 2,
        Attribute = 3,
        Operation = 4,
        Association = 5,
        StereotypeDefinition = 6,
        Folder = 7
    }
}