using System.Xml.Serialization;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.IArchitect.Agent.Persistence.Model.Settings
{
    [XmlRoot("config")]
    public class ApplicationDesignerSettingsPersistable
    {
        public const string FileExtension = "modeler.config";
        public const string ExtensionFileExtension = "modeler.extension.config";

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("displayOrder")]
        public int DisplayOrder { get; set; }

        [XmlElement("settings")]
        public DesignerSettingsPersistable Settings { get; set; }
    }
}