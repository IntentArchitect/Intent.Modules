using System;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

[Obsolete]
public class ElementCreationOptionOld : ContextMenuOption
{
    [XmlElement("specializationType")]
    public string SpecializationType { get; set; }

    [XmlElement("specializationTypeId")]
    public string SpecializationTypeId { get; set; }

    [XmlElement("defaultName")]
    public string DefaultName { get; set; }

    [XmlElement("allowMultiple")]
    public bool AllowMultiple { get; set; } = true;

    public override string ToString()
    {
        return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
               $"{nameof(Text)} = '{Text}'";
    }
}
