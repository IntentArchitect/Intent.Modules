using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class MappableElementsPackagePersistable
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlArray("imports")]
    [XmlArrayItem("import")]
    public List<MappableElementsPackageImportPersistable> PackageImports { get; set; } = new List<MappableElementsPackageImportPersistable>();

    [XmlArray("mappableElementSettings")]
    [XmlArrayItem("mappableElement")]
    public List<MappableElementSettingPersistable> MappableElements { get; set; } = new List<MappableElementSettingPersistable>();

    public List<MappableElementSettingPersistable> ResolveMappableElements(Dictionary<string, MappableElementsPackagePersistable> packagesRegistry)
    {
        var result = MappableElements.ToList();
        foreach (var import in PackageImports)
        {
            if (packagesRegistry.TryGetValue(import.Id, out var resolved))
            {
                result.AddRange(resolved.ResolveMappableElements(packagesRegistry));
            }
            else
            {
                throw new Exception($"Could not resolve import: {import.Name} [{import.Id}] for Mappable Element Package [{Name}]");
            }
        }

        return result;
    }
}

public class MappableElementsPackageImportPersistable
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }
}