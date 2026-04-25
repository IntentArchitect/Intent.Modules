using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model
{
    public class ApplicationTemplate_File
    {
        [XmlAttribute("src")]
        public string Src { get; set; }

        [XmlAttribute("target")]
        public ApplicationTemplate_FileTarget Target { get; set; } = ApplicationTemplate_FileTarget.ApplicationDirectory;

        [XmlAttribute("destination")]
        public string Destination { get; set; }

        [XmlAttribute("overwriteBehavior")]
        public ApplicationTemplate_FileOverwriteBehavior OverwriteBehavior { get; set; } = ApplicationTemplate_FileOverwriteBehavior.SkipIfExists;

        [XmlAttribute("applySubstitution")]
        public bool ApplySubstitution { get; set; } = true;
    }

    public enum ApplicationTemplate_FileTarget
    {
        [XmlEnum("ApplicationDirectory")]
        ApplicationDirectory = 1,

        [XmlEnum("OutputLocation")]
        OutputLocation = 2,

        [XmlEnum("MetadataFolder")]
        MetadataFolder = 3,
    }

    public enum ApplicationTemplate_FileOverwriteBehavior
    {
        [XmlEnum("SkipIfExists")]
        SkipIfExists = 1,

        [XmlEnum("Always")]
        Always = 2,
    }
}
