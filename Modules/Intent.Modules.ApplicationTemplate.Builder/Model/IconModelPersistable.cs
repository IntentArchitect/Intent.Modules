#nullable disable
using System.Xml.Serialization;
using Intent.Metadata.Models;

namespace Intent.Modules.ApplicationTemplate.Builder.Model
{
    public class IconModelPersistable
    {
        [XmlAttribute("type")]
        public IconType Type { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }
    }
}