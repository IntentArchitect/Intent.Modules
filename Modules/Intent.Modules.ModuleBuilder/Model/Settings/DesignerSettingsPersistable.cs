using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    [XmlRoot("settings")]
    public class DesignerSettingsPersistable
    {
        public const string FileExtension = "designer.settings";

        [XmlIgnore]
        public string Source { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; } = "3.1.1"; // this is in sync with the nuget package version

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

        [XmlArray("associationExtensions")]
        [XmlArrayItem("associationExtension")]
        public List<AssociationSettingExtensionPersistable> AssociationExtensions { get; set; } = new List<AssociationSettingExtensionPersistable>();

        [XmlArray("mappingSettings")]
        [XmlArrayItem("mappingSetting")]
        public List<AdvancedMappingSettingsPersistable> MappingSettings { get; set; } = new List<AdvancedMappingSettingsPersistable>();

        [XmlArray("mappableElementPackages")]
        [XmlArrayItem("mappableElementPackage")]
        public List<MappableElementsPackagePersistable> MappableElementPackages { get; set; } = new List<MappableElementsPackagePersistable>();

        [XmlArray("mappableElementPackageExtensions")]
        [XmlArrayItem("mappableElementPackageExtension")]
        public List<MappableElementsPackageExtensionPersistable> MappableElementPackageExtensions { get; set; } = new List<MappableElementsPackageExtensionPersistable>();
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
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        [XmlElement("hint")]
        public string Hint { get; set; }

        [XmlElement("isRequired")]
        public bool IsRequired { get; set; } = true;

        [XmlArray("targetTypes")]
        [XmlArrayItem("type")]
        public string[] TargetTypes { get; set; } = Array.Empty<string>();

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

        private bool _hasBeenAdjusted = false;
        public void AdjustFromExtension(TypeReferenceExtensionSettingPersistable extension)
        {
            var isGettingAdjusted = false;
            if (IsRequired != extension.IsRequired)
            {
                IsRequired = extension.IsRequired;
                isGettingAdjusted = true;
            }

            if (!string.IsNullOrWhiteSpace(extension.DisplayName))
            {
                DisplayName = extension.DisplayName;
                isGettingAdjusted = true;
            }

            if (!string.IsNullOrWhiteSpace(extension.Hint))
            {
                Hint = extension.Hint;
                isGettingAdjusted = true;
            }

            if (extension.TargetTypes != null && extension.TargetTypes.Any())
            {
                // Extend target types
                TargetTypes = TargetTypes.Concat(extension.TargetTypes).Distinct().ToArray();
                //isGettingAdjusted = true; (multiple extensions can extend these target types).
            }

            DefaultTypeId = extension.DefaultTypeId ?? DefaultTypeId;
            if (extension.AllowIsNavigable != BooleanExtensionOptions.Inherit)
            {
                AllowIsNavigable = extension.AllowIsNavigable == BooleanExtensionOptions.True;
                isGettingAdjusted = true;
            }
            if (extension.AllowIsNullable != BooleanExtensionOptions.Inherit)
            {
                AllowIsNullable = extension.AllowIsNullable == BooleanExtensionOptions.True;
                isGettingAdjusted = true;
            }
            if (extension.AllowIsCollection != BooleanExtensionOptions.Inherit)
            {
                AllowIsCollection = extension.AllowIsCollection == BooleanExtensionOptions.True;
                isGettingAdjusted = true;
            }

            if (isGettingAdjusted && _hasBeenAdjusted)
            {
                throw new InvalidOperationException("Type Reference Settings have already been adjusted by a previous extension. " +
                                                    "To ensure that designer settings are predictable, more than one " +
                                                    "extension cannot change these values.");
            }

            if (isGettingAdjusted)
            {
                _hasBeenAdjusted = true;
            }
        }
    }

    public class TypeOrderPersistable : IEquatable<TypeOrderPersistable>
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

        public bool Equals(TypeOrderPersistable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeOrderPersistable)obj);
        }

        public override int GetHashCode()
        {
            return (Type != null ? Type.GetHashCode() : 0);
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

        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<MacroPersistable> Macros { get; set; } = new List<MacroPersistable>();

        public void OnLoad(DesignerSettingsPersistable designerSettings)
        {
            foreach (var macro in Macros)
            {
                macro.Source = $"{designerSettings.Name} ({designerSettings.Source})";
            }
        }
    }

    public class ElementCreationOption : ContextMenuOption
    {
        [XmlAttribute("type")]
        public ElementType Type { get; set; }

        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("specializationTypeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

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
        StereotypeDefinition = 2,
        [XmlEnum("stereotype-property")]
        StereotypeProperty = 3
    }

    public abstract class ContextMenuOption
    {
        [XmlAttribute("order")]
        public string Order { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("shortcut")]
        public string Shortcut { get; set; }

        [XmlElement("macShortcut")]
        public string MacShortcut { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("isOptionVisibleFunction")]
        public string IsOptionVisibleFunction { get; set; }
    }

    public class MappingOption : ContextMenuOption
    {
        [XmlAttribute("mappingType")]
        public string MappingType { get; set; }

        [XmlAttribute("mappingTypeId")]
        public string MappingTypeId { get; set; }
    }

    public class RunScriptOption : ContextMenuOption
    {
        [XmlElement("script")]
        public string Script { get; set; }

        public override string ToString()
        {
            return $"{nameof(Text)} = '{Text}'";
        }
    }
}