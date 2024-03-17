using System;
using System.IO;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class PackageReferenceModel
    {
        private string _absolutePath;
        private string _relativePath;

        [XmlAttribute("packageId")]
        public string PackageId { get; set; }

        [XmlAttribute("include")]
        public string Name { get; set; }

        [XmlAttribute("isExternal")]
        public bool IsExternal { get; set; }

        [XmlElement("module")]
        public string Module { get; set; }

        [XmlElement("path")]
        public string RelativePath
        {
            get => _relativePath;
            set => _relativePath = value;
        }

        [XmlIgnore]
        public string AbsolutePath
        {
            get => _absolutePath;
            set => _absolutePath = value;
        }
    }
}