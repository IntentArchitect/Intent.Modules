#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class SettingFieldOptionPersistable
{
    [XmlAttribute("value")]
    public string Value { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; }
}