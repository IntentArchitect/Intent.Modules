using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class MappableElementsPackageExtensionPersistable
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("extendPackageId")]
    public string ExtendPackageId { get; set; }

    [XmlAttribute("extendPackage")]
    public string ExtendPackage { get; set; }

    [XmlArray("imports")]
    [XmlArrayItem("import")]
    public List<MappableElementsPackageImportPersistable> PackageImports { get; set; } = new List<MappableElementsPackageImportPersistable>();
    public bool ShouldSerializePackageImports() => PackageImports.Count != 0;

    [XmlArray("mappableElementSettings")]
    [XmlArrayItem("mappableElement")]
    public List<MappableElementSettingPersistable> MappableElements { get; set; } = new List<MappableElementSettingPersistable>();
    public bool ShouldSerializeMappableElements() => MappableElements.Count != 0;

    [XmlArray("mappableElementExtensions")]
    [XmlArrayItem("mappableElementExtension")]
    public List<MappableElementExtensionSettingsPersistable> MappableElementExtensions { get; set; } = [];
    public bool ShouldSerializeMappableElementExtensions() => MappableElementExtensions.Count != 0;
}