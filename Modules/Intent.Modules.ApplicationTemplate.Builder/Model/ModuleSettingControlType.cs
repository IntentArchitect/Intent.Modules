using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public enum ModuleSettingControlType
{
    [XmlEnum("hidden")]
    Hidden = -1,

    [XmlEnum("text")]
    TextBox = 0,

    [XmlEnum("number")]
    Number = 1,

    [XmlEnum("checkbox")]
    Checkbox = 2,

    [XmlEnum("switch")]
    Switch = 3,

    [XmlEnum("textarea")]
    TextArea = 4,

    [XmlEnum("select")]
    Select = 5,

    [XmlEnum("multi-select")]
    MultiSelect = 6,
}