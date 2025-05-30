﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class MappableElementSettingPersistable
{
    public const string SyncWithValue = "value";
    public const string SyncWithStereotypeProperty = "stereotype-property";

    private bool? _isTraversable;

    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("type")]
    public string SpecializationType { get; set; }

    [XmlAttribute("typeId")]
    public string SpecializationTypeId { get; set; }

    [XmlAttribute("version")]
    public string Version { get; set; } = "1.0.0";

    [XmlElement("displayFunction")]
    public string DisplayFunction { get; set; }

    [XmlElement("filterFunction")]
    public string FilterFunction { get; set; }

    [XmlElement("isRequiredFunction")]
    public string IsRequiredFunction { get; set; }

    [XmlElement("mustBeMappedBeforeChildren")]
    public bool? MustBeMappedBeforeChildren { get; set; }
    public bool ShouldSerializeMustBeMappedBeforeChildren() => MustBeMappedBeforeChildren.HasValue;

    [XmlElement("allowMultipleMappings")]
    public bool? AllowMultipleMappings { get; set; }

    [XmlElement("isMappableFunction")]
    public string IsMappableFunction { get; set; }

    [XmlElement("isTraversable")]
    public bool IsTraversable
    {
        get => _isTraversable ?? TraversableTypes.Any();
        set => _isTraversable = value;
    }

    [XmlElement("getTraversableTypeFunction")]
    public string GetTraversableTypeFunction { get; set; }

    [XmlArray("traversableTypes")]
    [XmlArrayItem("mappingId")]
    public List<string> TraversableTypes { get; set; } = new();

    [XmlElement("overrideTypeReferenceFunction")]
    public string OverrideTypeReferenceFunction { get; set; }
    public bool ShouldSerializeOverrideTypeReferenceFunction() => !string.IsNullOrWhiteSpace(OverrideTypeReferenceFunction);

    [XmlElement("canBeModified")]
    public bool CanBeModified { get; set; }

    [XmlElement("createNameFunction")]
    public string CreateNameFunction { get; set; }

    [XmlElement("useChildSettingsFrom")]
    public string UseChildSettingsFrom { get; set; }

    [XmlElement("overrideMapToParentFunction")]
    public string OverrideMapToParentFunction { get; set; }
    public bool ShouldSerializeOverrideMapToParentFunction() => !string.IsNullOrWhiteSpace(OverrideMapToParentFunction);

    [XmlElement("represents")]
    public string Represents { get; set; }

    [XmlElement("iconOverride")]
    public IconModelPersistable IconOverride { get; set; }

    [XmlElement("syncWith")]
    public string SyncWith { get; set; }

    [XmlElement("syncStereotypeId")]
    public string SyncStereotypeId { get; set; }
    public bool ShouldSerializeSyncStereotypeId() => SyncWith == SyncWithStereotypeProperty;

    [XmlElement("syncStereotypePropertyId")]
    public string SyncStereotypePropertyId { get; set; }
    public bool ShouldSerializeSyncStereotypePropertyId() => SyncWith == SyncWithStereotypeProperty;

    [XmlElement("onMappingsChangedScript")]
    public string OnMappingsChangedScript { get; set; }
    public bool ShouldSerializeOnMappingsChangedScript() => !string.IsNullOrWhiteSpace(OnMappingsChangedScript);

    [XmlElement("validateFunction")]
    public string ValidateFunction { get; set; }
    public bool ShouldSerializeValidateFunction() => !string.IsNullOrWhiteSpace(ValidateFunction);

    [XmlArray("scriptOptions")]
    [XmlArrayItem("option")]
    public List<RunScriptOption> ScriptOptions { get; set; }
    public bool ShouldSerializeScriptOptions() => ScriptOptions.Count != 0;

    [XmlArray("staticMappableSettings")]
    [XmlArrayItem("staticMappableSetting")]
    public List<MappableElementSettingPersistable> StaticMappableSettings { get; set; }
    public bool ShouldSerializeStaticMappableSettings() => StaticMappableSettings.Count != 0;

    [XmlArray("childSettings")]
    [XmlArrayItem("childSetting")]
    public List<MappableElementSettingPersistable> ChildSettings { get; set; }
    public bool ShouldSerializeChildSettings() => ChildSettings.Count != 0;
}


public class MappableElementExtensionSettingsPersistable
{

    [XmlElement("validateFunction")]
    public string ValidateFunction { get; set; }
    public bool ShouldSerializeValidateFunction() => !string.IsNullOrWhiteSpace(ValidateFunction);

    [XmlArray("targetSettings")]
    [XmlArrayItem("type")]
    public List<TargetTypePersistable> TargetSettings { get; set; } = [];

    [XmlArray("scriptOptions")]
    [XmlArrayItem("option")]
    public List<RunScriptOption> ScriptOptions { get; set; } = [];

    [XmlArray("staticMappableSettings")]
    [XmlArrayItem("staticMappableSetting")]
    public List<MappableElementSettingPersistable> StaticMappableSettings { get; set; } = [];

    [XmlArray("childSettings")]
    [XmlArrayItem("childSetting")]
    public List<MappableElementSettingPersistable> ChildSettings { get; set; } = [];
}