#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public enum ComponentGroupSelectionMode
{
    [XmlEnum(Name = "allow-multiple")]
    AllowSelectMultiple = 0,

    [XmlEnum(Name = "allow-single-only")]
    AllowSingleOnly = 1,
}