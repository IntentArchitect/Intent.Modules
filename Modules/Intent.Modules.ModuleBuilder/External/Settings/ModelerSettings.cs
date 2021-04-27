using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    [XmlRoot("settings")]
    public class DesignerSettingsPersistable
    {
        public const string FileExtension = "designer.settings";

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlArray("designerReferences")]
        [XmlArrayItem("designerReference")]
        public List<DesignerSettingsReference> DesignerReferences { get; set; } = new List<DesignerSettingsReference>();

        [XmlArray("packageSettings")]
        [XmlArrayItem("packageSetting")]
        public List<PackageSettingsPersistable> PackageSettings { get; set; } = new List<PackageSettingsPersistable>();

        [XmlArray("packageExtensions")]
        [XmlArrayItem("packageExtension")]
        public List<PackageSettingsExtensionPersistable> PackageExtensions { get; set; } = new List<PackageSettingsExtensionPersistable>();

        [XmlArray("elementSettings")]
        [XmlArrayItem("elementSetting")]
        public List<ElementSettingPersistable> ElementSettings { get; set; } = new List<ElementSettingPersistable>();

        [XmlArray("elementExtensions")]
        [XmlArrayItem("elementExtension")]
        public List<ElementSettingExtensionPersistable> ElementExtensions { get; set; } = new List<ElementSettingExtensionPersistable>();

        [XmlArray("associationSettings")]
        [XmlArrayItem("associationSetting")]
        public List<AssociationSettingsPersistable> AssociationSettings { get; set; } = new List<AssociationSettingsPersistable>();

    }

    public class StereotypeSettingsPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlArray("targetTypeOptions")]
        [XmlArrayItem("option")]
        public List<TargetTypeOption> TargetTypeOptions { get; set; }
    }

    public class TargetTypeOption
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

    public class PackageSettingsExtensionPersistable
    {
        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlArray("requiredPackages")]
        [XmlArrayItem("package")]
        public string[] RequiredPackages { get; set; } = new string[0];

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<ElementMacroPersistable> Macros { get; set; } = new List<ElementMacroPersistable>();
    }

    public class ElementCreationOption
    {
        [XmlAttribute("order")]
        public string Order { get; set; }

        [XmlAttribute("type")]
        public ElementType Type { get; set; }

        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("specializationTypeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("shortcut")]
        public string Shortcut { get; set; }

        [XmlElement("macShortcut")]
        public string MacShortcut { get; set; }

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
        [XmlEnum("n/a")]
        NotApplicable = -1,
        [XmlEnum("element")]
        Element = 0,
        [XmlEnum("association")]
        Association = 1,
        [XmlEnum("stereotype-definition")]
        StereotypeDefinition = 2
    }
}