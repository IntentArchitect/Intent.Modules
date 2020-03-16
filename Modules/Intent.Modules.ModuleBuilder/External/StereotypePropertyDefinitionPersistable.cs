using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class StereotypePropertyDefinitionPersistable
    {
        private StereotypePropertyControlType? _controlType;
        private StereotypePropertyOptionsSource? _optionsSource;

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("controlType")]
        public StereotypePropertyControlType ControlType { get; set; }

        [XmlElement("optionsSource")]
        public StereotypePropertyOptionsSource OptionsSource { get; set; }

        [XmlElement("placeholder")]
        public string Placeholder { get; set; }

        [XmlElement("defaultValue")]
        public string DefaultValue { get; set; }

        [XmlArray("valueOptions")]
        [XmlArrayItem("option")]
        public List<string> ValueOptions { get; set; }

        [XmlArray("lookupTypes")]
        [XmlArrayItem("type")]
        public List<string> LookupTypes { get; set; }

        [XmlElement("lookupInternalTargetPropertyId")]
        public string LookupInternalTargetPropertyId { get; set; }

        [XmlElement("isActiveFunction")]
        public string IsActiveFunction { get; set; }

        [XmlElement("isRequiredFunction")]
        public string IsRequiredFunction { get; set; }

        public override string ToString()
        {
            return $"{nameof(StereotypePropertyDefinitionPersistable)}: {Name}";
        }
    }
}