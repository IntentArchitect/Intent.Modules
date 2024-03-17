using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class TypeReferenceExtensionSettingPersistable
    {
        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        [XmlElement("hint")]
        public string Hint { get; set; }

        [XmlElement("isRequired")]
        public bool IsRequired { get; set; }

        [XmlArray("targetTypes")]
        [XmlArrayItem("type")]
        public string[] TargetTypes { get; set; }

        [XmlElement("defaultTypeId")]
        public string DefaultTypeId { get; set; }

        [XmlElement("allowIsNavigable")]
        public BooleanExtensionOptions AllowIsNavigable { get; set; } = BooleanExtensionOptions.Inherit;

        [XmlElement("allowIsNullable")]
        public BooleanExtensionOptions AllowIsNullable { get; set; } = BooleanExtensionOptions.Inherit;

        [XmlElement("allowIsCollection")]
        public BooleanExtensionOptions AllowIsCollection { get; set; } = BooleanExtensionOptions.Inherit;
    }

    public enum BooleanExtensionOptions
    {
        [XmlEnum("inherit")]
        Inherit = 0,
        [XmlEnum("true")]
        True = 1,
        [XmlEnum("false")]
        False = 2
    }
}