using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    [XmlRoot("config")]
    public class ApplicationDesignerPersistable
    {
        public const string FileExtension = "designer.config";

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("order")]
        public int Order { get; set; }

        private IconModelPersistable _icon;
        [XmlElement("icon")]
        public IconModelPersistable Icon
        {
            get
            {
                if (_icon != null)
                {
                    return _icon;
                }

                return (_icon = new IconModelPersistable { Type = IconType.FontAwesome, Source = "question-circle" });
            }
            set => _icon = value;
        }

        [XmlElement("loadStartPage")]
        public bool LoadStartPage { get; set; }

        [XmlElement("outputConfiguration")]
        public DesignerOutputConfiguration OutputConfiguration { get; set; }

        [XmlArray("designerReferences")]
        [XmlArrayItem("designerReference")]
        public List<DesignerSettingsReference> DesignerReferences { get; set; } = new List<DesignerSettingsReference>();

        [XmlArray("packageReferences")]
        [XmlArrayItem("packageReference")]
        public List<PackageReferenceModel> PackageReferences { get; set; } = new List<PackageReferenceModel>();

    }
}
