using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.ModelerBuilder.External
{
    [XmlRoot("config")]
    public class ApplicationModelerModel
    {
        public ApplicationModelerModel()
        {
            PackageReferences = new List<string>();
        }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("loadStartPage")]
        public bool LoadStartPage { get; set; }

        [XmlElement("settings")]
        public ModelerSettingsPersistable Settings { get; set; }

        [XmlArray("packages")]
        [XmlArrayItem("package")]
        public List<string> PackageReferences { get; set; }
    }
}
