using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class MappingSettingsPersistable
    {
        [XmlElement("defaultModeler")]
        public string DefaultModeler { get; set; }

        [XmlElement("optionsSource")]
        public MappingOptionsSource OptionsSource { get; set; }

        [XmlElement("lookupElementFunction")]
        public string LookupElementFunction { get; set; }

        [XmlElement("autoSyncTypeReferences")]
        public bool? AutoSyncTypeReferences { get; set; }

        [XmlArray("targetTypeOptions")]
        [XmlArrayItem("option")]
        public List<TargetTypeOption> LookupTypes { get; set; }

        [XmlElement("mapFrom")]
        public MappingMapFrom MapFrom { get; set; }

        [XmlArray("mappings")]
        [XmlArrayItem("mapping")]
        public List<ElementMappingSettingPersistable> MappedTypes { get; set; }
    }

    public class ElementMappingSettingPersistable
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("criteria")]
        public ElementMappingCriteriaSettingPersistable Criteria { get; set; } = new ElementMappingCriteriaSettingPersistable();

        [XmlElement("mapTo")]
        public ElementMappingMapToSettingPersistable MapTo { get; set; } = new ElementMappingMapToSettingPersistable();

        [XmlElement("behaviour")]
        public ElementMappingBehaviourPersistable Behaviour { get; set; } = new ElementMappingBehaviourPersistable();

        [XmlArray("childMappings")]
        [XmlArrayItem("mapping")]
        public List<ElementMappingSettingPersistable> ChildMappingSettings { get; set; } = new List<ElementMappingSettingPersistable>();
    }

    public class ElementMappingCriteriaSettingPersistable : IEquatable<ElementMappingCriteriaSettingPersistable>
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("hasTypeReference")]
        public bool? HasTypeReference { get; set; }

        [XmlElement("isCollection")]
        public bool? IsCollection { get; set; }

        [XmlElement("hasChildren")]
        public bool? HasChildren { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} : {SpecializationType}{Environment.NewLine}" +
                   $"{nameof(HasTypeReference)} : {HasTypeReference.ToString() ?? "null"}{Environment.NewLine}" +
                   $"{nameof(IsCollection)} : {IsCollection.ToString() ?? "null"}{Environment.NewLine}" +
                   $"{nameof(HasChildren)} : {HasChildren.ToString() ?? "null"}{Environment.NewLine}";
        }

        public bool Equals(ElementMappingCriteriaSettingPersistable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(SpecializationType, other.SpecializationType) && HasTypeReference == other.HasTypeReference && IsCollection == other.IsCollection && HasChildren == other.HasChildren;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementMappingCriteriaSettingPersistable)obj);
        }

        public override int GetHashCode()
        {
            return SpecializationType.GetHashCode();
        }
    }

    public class ElementMappingMapToSettingPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlAttribute("childMappingMode")]
        public ChildMappingMode ChildMappingMode { get; set; }

        [XmlAttribute("useMappingSettings")]
        public string UseMappingSettings { get; set; }

        [XmlElement("typeReferenceCreation")]
        public ElementMappingTypeCreationSettingPersistable TypeReferenceCreation { get; set; }
    }

    public class ElementMappingBehaviourPersistable
    {
        [XmlAttribute("autoSelectChildren")]
        public bool AutoSelectChildren { get; set; }
    }

    public enum ChildMappingMode
    {
        [XmlEnum("map-to-child")]
        MapToChild = 0,
        [XmlEnum("traverse")]
        Traverse = 1
    }

    public enum MappingOptionsSource
    {
        Unknown = 0,

        [XmlEnum("elements-of-type")]
        ElementsOfType = 1,

        [XmlEnum("lookup-element")]
        LookupElement = 2
    }

    public enum MappingMapFrom
    {
        Unknown = 0,

        [XmlEnum("root")]
        RootElement = 1,
        [XmlEnum("children")]
        ChildElements = 2
    }

    public class ElementMappingTypeCreationSettingPersistable
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("nameFunction")]
        public string NameFunction { get; set; }
    }
}