#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateDefaults
{
    [XmlElement("name")]
    public string Name { get; set; } = "NewApplication";

    [XmlElement("relativeOutputLocation")]
    public string RelativeOutputLocation { get; set; } = string.Empty;

    /// <summary>
    /// The inverse of the designer's "Metadata in Subfolder" default.
    /// </summary>
    [XmlElement("placeInSameDirectory")]
    public bool PlaceInSameDirectory { get; set; }

    [XmlElement("setGitIgnoreEntries")]
    public bool SetGitIgnoreEntries { get; set; }
}
