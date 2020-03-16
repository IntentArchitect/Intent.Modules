using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public enum StereotypePropertyType
    {
        [XmlEnum("string")]
        String = 0,
        [XmlEnum("number")]
        Number = 1,
        [XmlEnum("boolean")]
        Boolean = 2,
        [XmlEnum("options")]
        Options = 3,
        [XmlEnum("text")]
        Text = 4,
        [XmlEnum("lookup-type")]
        LookupType = 5,
        [XmlEnum("internal-lookup")]
        InternalLookup = 6,
    }

    public enum StereotypePropertyControlType

    {
        [XmlEnum("text")]
        TextBox = 0,

        [XmlEnum("number")]
        Number = 1,

        [XmlEnum("checkbox")]
        Checkbox = 2,

        [XmlEnum("textarea")]
        TextArea = 3,

        [XmlEnum("select")]
        Select = 4,

        [XmlEnum("multi-select")]
        MultiSelect = 5,
    }

    public enum StereotypePropertyOptionsSource
    {
        [XmlEnum("n/a")]
        NotApplicable = 0,

        [XmlEnum("options")]
        Options = 1,

        [XmlEnum("lookup-element")]
        LookupElement = 2,

        [XmlEnum("nested-lookup")]
        NestedLookup  = 3
    }
}