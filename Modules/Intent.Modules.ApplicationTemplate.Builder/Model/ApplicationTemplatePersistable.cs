#nullable disable
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model
{
    [XmlRoot("applicationTemplate")]
    public class ApplicationTemplatePersistable
    {
        [XmlAttribute("type")]
        public TemplateType TemplateType { get; set; } = TemplateType.ApplicationTemplate;

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("supportedClientVersions")]
        public string SupportedClientVersions { get; set; }

        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        [XmlElement("coverImageUrl")]
        public ApplicationTemplate_Image CoverImageUrl { get; set; }

        [XmlArray("additionalImages")]
        [XmlArrayItem("image")]
        public List<ApplicationTemplate_Image> AdditionalImages { get; set; } = [];

        public bool ShouldSerializeAdditionalImages() => AdditionalImages.Count > 0;

        [XmlElement("shortDescription")]
        public string ShortDescription { get; set; }

        [XmlElement("longDescription")]
        public string LongDescription { get; set; }
        public bool ShouldSerializeLongDescription() => !string.IsNullOrWhiteSpace(LongDescription);

        [XmlElement("authors")]
        public string Authors { get; set; }

        [XmlElement("priority")]
        public int Priority { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlElement("projectUrl")]
        public string ProjectUrl { get; set; }

        [XmlElement("tags")]
        public string Tags { get; set; }

        [XmlElement("defaults")]
        public ApplicationTemplateDefaults Defaults { get; set; }

        [XmlArray("componentGroups")]
        [XmlArrayItem("group")]
        public List<ApplicationTemplateComponentGroup> ComponentGroups { get; set; }

        [XmlArray("additionalSettings")]
        [XmlArrayItem("group")]
        public List<SettingsGroupPersistable> AdditionalSettings { get; set; }

        /// <summary>
        /// The minimum version to install for a module if it's required as a dependency (including
        /// interops) and not otherwise directly specified.
        /// </summary>
        [XmlArray("minimumDependencyVersions")]
        [XmlArrayItem("module")]
        public List<MinimumDependencyVersion> MinimumDependencyVersions { get; set; }
        public bool ShouldSerializeMinimumDependencyVersions() => MinimumDependencyVersions?.Count > 0;
    }

    public class ApplicationTemplate_Image
    {
        //[XmlAttribute("href")]
        //public string Href { get; set; } // to open external links (To Do)

        [XmlAttribute("src")]
        public string Src { get; set; } // url of image
    }
    public enum TemplateType
    {
        [XmlEnum("application-template")]
        ApplicationTemplate = 1,
        [XmlEnum("module-building")]
        ModuleBuilding = 2,
        [XmlEnum("sample-solution")]
        SampleSolution = 3,

    }
}
