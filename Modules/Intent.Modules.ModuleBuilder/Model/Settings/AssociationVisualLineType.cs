using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public enum AssociationVisualLineType
{
    [XmlEnum(Name = "Elbow-Connector")]
    ElbowConnector = 0,
    [XmlEnum(Name = "Curved")]
    Curved = 1,
}