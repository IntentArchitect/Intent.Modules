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
        public List<DesignerSettingsReference> DesignerReferences { get; set; } = [];

        [XmlArray("packageSettings")]
        [XmlArrayItem("packageSetting")]
        public List<PackageSettingsPersistable> PackageSettings { get; set; } = [];

        [XmlArray("packageExtensions")]
        [XmlArrayItem("packageExtension")]
        public List<PackageSettingsExtensionPersistable> PackageExtensions { get; set; } = [];

        [XmlArray("elementSettings")]
        [XmlArrayItem("elementSetting")]
        public List<ElementSettingPersistable> ElementSettings { get; set; } = [];

        [XmlArray("elementExtensions")]
        [XmlArrayItem("elementExtension")]
        public List<ElementSettingExtensionPersistable> ElementExtensions { get; set; } = [];

        [XmlArray("associationSettings")]
        [XmlArrayItem("associationSetting")]
        public List<AssociationSettingsPersistable> AssociationSettings { get; set; } = [];

        [XmlArray("associationExtensions")]
        [XmlArrayItem("associationExtension")]
        public List<AssociationSettingExtensionPersistable> AssociationExtensions { get; set; } = [];

        [XmlArray("mappingSettings")]
        [XmlArrayItem("mappingSetting")]
        public List<AdvancedMappingSettingsPersistable> MappingSettings { get; set; } = [];

        [XmlArray("mappableElementPackages")]
        [XmlArrayItem("mappableElementPackage")]
        public List<MappableElementsPackagePersistable> MappableElementPackages { get; set; } = [];

        [XmlArray("mappableElementPackageExtensions")]
        [XmlArrayItem("mappableElementPackageExtension")]
        public List<MappableElementsPackageExtensionPersistable> MappableElementPackageExtensions { get; set; } = [];

        [XmlArray("suggestionSettings")]
        [XmlArrayItem("suggestionSetting")]
        public List<SuggestionSettingPersistable> SuggestionSettings { get; set; } = [];
        public bool ShouldSerializeSuggestionSettings() => SuggestionSettings.Any();

        [XmlArray("scripts")]
        [XmlArrayItem("script")]
        public List<DesignerScriptPersistable> Scripts { get; set; } = [];
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
        public List<TypeOrderPersistable> TypeOrder { get; set; }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<MacroPersistable> Macros { get; set; } = new List<MacroPersistable>();

        [XmlArray("childElementExtensions")]
        [XmlArrayItem("childElementExtension")]
        public ElementSettingExtensionPersistable[] ChildElementExtensions { get; set; } = [];
        public bool ShouldSerializeChildElementExtensions() => ChildElementExtensions.Any();
    }

    public class ElementCreationOption : ContextMenuOption
    {
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

    public class StereotypeCreationOption : ContextMenuOption
    {
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

    public enum ContextMenuOptionType
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
        StereotypeProperty = 3,
        [XmlEnum("run-script")] 
        RunScript = 4,
        [XmlEnum("open-advanced-mapping")] 
        OpenAdvancedMapping = 5,
    }

    public class AssociationCreationOption : ContextMenuOption
    {
        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("specializationTypeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("allowMultiple")]
        public bool AllowMultiple { get; set; } = true;

        [XmlElement("sourceEndDefault")]
        public AssociationEndCreationDefaults SourceEndDefaults { get; set; }

        [XmlElement("targetEndDefaults")]
        public AssociationEndCreationDefaults TargetEndDefaults { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }

    public class AssociationEndCreationDefaults
    {
        [XmlAttribute("isNavigable")]
        public bool IsNavigable { get; set; } = true;

        [XmlAttribute("isNullable")]
        public bool IsNullable { get; set; } = false;

        [XmlAttribute("isCollection")]
        public bool IsCollection { get; set; } = false;
    }

    public abstract class ContextMenuOption
    {
        [XmlAttribute("group")]
        public required int MenuGroup { get; set; }
        public bool ShouldSerializeMenuGroup() => MenuGroup > 0;

        [XmlAttribute("order")]
        public required string Order { get; set; }
        public bool ShouldSerializeOrder() => !string.IsNullOrWhiteSpace(Order);

        [XmlAttribute("type")]
        public required ContextMenuOptionType Type { get; set; }

        [XmlElement("text")]
        public required string Text { get; set; }

        [XmlElement("shortcut")]
        public required string Shortcut { get; set; }

        [XmlElement("macShortcut")]
        public required string MacShortcut { get; set; }
        public bool ShouldSerializeMacShortcut() => !string.IsNullOrWhiteSpace(MacShortcut);

        [XmlElement("triggerOnDoubleClick")]
        public bool? TriggerOnDoubleClick { get; set; }
        public bool ShouldSerializeTriggerOnDoubleClick() => TriggerOnDoubleClick.HasValue && TriggerOnDoubleClick.Value;

        [XmlElement("icon")]
        public required IconModelPersistable Icon { get; set; }

        [XmlElement("isOptionVisibleFunction")]
        public required string IsOptionVisibleFunction { get; set; }

        [XmlElement("topDivider")]
        public required bool HasTopDivider { get; set; }
        public bool ShouldSerializeHasTopDivider() => HasTopDivider;

        [XmlElement("bottomDivider")]
        public required bool HasBottomDivider { get; set; }
        public bool ShouldSerializeHasBottomDivider() => HasBottomDivider;

        [XmlArray("subMenuOptions")]
        [XmlArrayItem("createElement", typeof(ElementCreationOption))]
        [XmlArrayItem("createAssociation", typeof(AssociationCreationOption))]
        [XmlArrayItem("createStereotype", typeof(StereotypeCreationOption))]
        [XmlArrayItem("runScript", typeof(RunScriptOption))]
        [XmlArrayItem("openMapping", typeof(MappingOption))]
        public List<ContextMenuOption> SubMenuOptions { get; set; } = [];
        public bool ShouldSerializeSubMenuOptions() => SubMenuOptions?.Any() == true;
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