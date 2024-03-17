using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class AdvancedMappingSettingsPersistable
{
    [XmlAttribute("type")]
    public string MappingType { get; set; }

    [XmlAttribute("typeId")]
    public string MappingTypeId { get; set; }

    [XmlElement("title")]
    public string Title { get; set; }

    [XmlElement("isRequiredFunction")] 
    public string IsRequiredFunction { get; set; }

    [XmlElement("sourceRootElementFunction")]
    public string SourceRootElementFunction { get; set; }

    [XmlElement("targetRootElementFunction")]
    public string TargetRootElementFunction { get; set; }

    [XmlArray("sourceMappings")]
    [XmlArrayItem("importSettings", typeof(MappableElementsPackageImportPersistable))]
    [XmlArrayItem("reference", typeof(MappableElementSettingIdentifierPersistable))]
    [XmlArrayItem("mapping", typeof(MappableElementSettingPersistable))]
    public List<object> SourceMappableSettings { get; set; }

    [XmlIgnore]
    public List<MappableElementSettingPersistable> SourceSettings { get; set; }

    [XmlArray("targetMappings")]
    [XmlArrayItem("importSettings", typeof(MappableElementsPackageImportPersistable))]
    [XmlArrayItem("reference", typeof(MappableElementSettingIdentifierPersistable))]
    [XmlArrayItem("mapping", typeof(MappableElementSettingPersistable))]
    public List<object> TargetMappableSettings { get; set; }

    [XmlIgnore]
    public List<MappableElementSettingPersistable> TargetSettings { get; set; }

    [XmlArray("mappingTypes")]
    [XmlArrayItem("mappingType")]
    public List<AdvancedMappingTypePersistable> MappingTypes { get; set; }

    /// <summary>
    /// Replaces references with the resolved common mappable elements:
    /// </summary>
    /// <param name="packageRegistry"></param>
    /// <param name="packageExtensionRegistry"></param>
    /// <exception cref="Exception"></exception>
    public List<MappableElementSettingPersistable> ResolveSourceMappableElements(Dictionary<string, MappableElementsPackagePersistable> packageRegistry, Dictionary<string, List<MappableElementsPackageExtensionPersistable>> packageExtensionRegistry)
    {
        var result = new List<MappableElementSettingPersistable>();
        result.AddRange(SourceMappableSettings.OfType<MappableElementSettingPersistable>());

        foreach (var import in SourceMappableSettings.OfType<MappableElementsPackageImportPersistable>().ToList())
        {
            if (packageRegistry.TryGetValue(import.Id, out var resolved))
            {
                result.AddRange(resolved.ResolveMappableElements(packageRegistry));
            }
            else
            {
                throw new Exception($"Could not resolve Source Mappable Element reference: {import.Name} [{import.Id}] for Advance Mapping Setting [{MappingType}]");
            }

            if (packageExtensionRegistry.TryGetValue(import.Id, out var extensions))
            {
                foreach (var extension in extensions)
                {
                    result.AddRange(extension.ResolveMappableElements(packageRegistry));
                }
            }
        }
        return result;
    }

    public List<MappableElementSettingPersistable> ResolveTargetMappableElements(Dictionary<string, MappableElementsPackagePersistable> packageRegistry, Dictionary<string, List<MappableElementsPackageExtensionPersistable>> packageExtensionRegistry)
    {
        var result = new List<MappableElementSettingPersistable>();
        result.AddRange(TargetMappableSettings.OfType<MappableElementSettingPersistable>());

        foreach (var import in TargetMappableSettings.OfType<MappableElementsPackageImportPersistable>().ToList())
        {
            if (packageRegistry.TryGetValue(import.Id, out var resolved))
            {
                result.AddRange(resolved.ResolveMappableElements(packageRegistry));
            }
            else
            {
                throw new Exception($"Could not resolve Source Mappable Element reference: {import.Name} [{import.Id}] for Advance Mapping Setting [{MappingType}]");
            }

            if (packageExtensionRegistry.TryGetValue(import.Id, out var extensions))
            {
                foreach (var extension in extensions)
                {
                    result.AddRange(extension.ResolveMappableElements(packageRegistry));
                }
            }
        }
        return result;
    }
}