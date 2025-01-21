#nullable disable
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class ApplicationTemplateDefaults
{
    [XmlElement("name")]
    public string Name { get; set; } = "NewApplication";

    [XmlElement("relativeOutputLocation")]
    public string RelativeOutputLocation { get; set; } = string.Empty;

    [XmlElement("placeInSameDirectory")]
    public bool PlaceInSameDirectory { get; set; }

    [XmlElement("separateIntentFiles")]
    public bool SeparateIntentFiles { get; set; }

    [XmlElement("setGitIgnoreEntries")]
    public bool SetGitIgnoreEntries { get; set; }

    /// <summary>
    /// Has default value of <see langword="true"/> for backwards compatibility.
    /// </summary>
    [XmlElement("createFolderForSolution")]
    public bool CreateFolderForSolution { get; set; } = true;
}