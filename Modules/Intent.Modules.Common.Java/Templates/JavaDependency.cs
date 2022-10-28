using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java.Templates
{
    /// <summary>
    /// Details of a Java Maven dependency.
    /// </summary>
    public class JavaDependency
    {
        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        public JavaDependency(string groupId, string artifactId, string version, bool optional)
        {
            GroupId = groupId;
            ArtifactId = artifactId;
            Version = version;
            Optional = optional;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Make a single constructor where version has a default value of null and optional has a default value of false")]
        public JavaDependency(string groupId, string artifactId, string version = null) : this(groupId, artifactId, version, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Make a single constructor where version has a default value of null and optional has a default value of false")]
        public JavaDependency(string groupId, string artifactId, bool optional) : this(groupId, artifactId, null, optional)
        {
        }

        /// <summary>
        /// The groupId of the dependency.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The groupId of the dependency.
        /// </summary>
        public string ArtifactId { get; set; }

        /// <summary>
        /// The version of the dependency.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Whether or not the dependency is optional.
        /// </summary>
        public bool Optional { get; set; }
    }
}