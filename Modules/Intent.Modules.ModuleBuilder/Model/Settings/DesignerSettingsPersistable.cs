using Intent.ModuleBuilder.Api;
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

        [XmlArray("scripts")]
        [XmlArrayItem("script")]
        public List<DesignerScriptPersistable> Scripts { get; set; } = new List<DesignerScriptPersistable>();
    }

    public class DesignerScriptPersistable
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlArray("dependencies")]
        [XmlArrayItem("dependency")]
        public List<TargetReferencePersistable> Dependencies { get; set; }
        public bool ShouldSerializeDependencies() => Dependencies.Any();

        [XmlElement("script")]
        public string Script { get; set; }
        public bool ShouldSerializeScript() => !string.IsNullOrWhiteSpace(Script);

        [XmlElement("filePath")]
        public string FilePath { get; set; }
        public bool ShouldSerializeFilePath() => !string.IsNullOrWhiteSpace(FilePath);
    }

    public class TargetReferencePersistable
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
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
        [XmlElement("scriptReference")]
        public TargetReferencePersistable ScriptReference { get; set; }
        public bool ShouldSerializeScriptReference() => ScriptReference != null;

        [XmlArray("dependencies")]
        [XmlArrayItem("dependency")]
        public List<TargetReferencePersistable> Dependencies { get; set; }
        public bool ShouldSerializeDependencies() => Dependencies?.Any() == true;

        [XmlElement("script")]
        public string Script { get; set; }
        public bool ShouldSerializeScript() => !string.IsNullOrWhiteSpace(Script);

        public override string ToString()
        {
            return $"{nameof(Text)} = '{Text}'";
        }
    }
}