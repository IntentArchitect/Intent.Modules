using System.Collections.Generic;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java.Templates
{
    /// <summary>
    /// Details of a Java Maven dependency. See <see href="https://maven.apache.org/pom.html#dependencies"/> for details on various properties.
    /// </summary>
    public class JavaDependency
    {
        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Make the \"version\" parameter have a default value of null and remove other constructors.")]
        public JavaDependency(
            string groupId,
            string artifactId,
            string version,
            List<JavaDependencyExclusion> exclusions = null,
            string type = null,
            JavaDependencyScope? scope = null,
            bool optional = false)
        {
            GroupId = groupId;
            ArtifactId = artifactId;
            Version = version;
            Exclusions = exclusions ?? new List<JavaDependencyExclusion>();
            Type = type;
            Scope = scope;
            Optional = optional;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload")]
        public JavaDependency(string groupId, string artifactId, string version, bool optional)
            : this(
                groupId: groupId,
                artifactId: artifactId,
                version: version,
                exclusions: null,
                type: null,
                scope: null,
                optional: optional)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload")]
        public JavaDependency(string groupId, string artifactId, string version = null)
            : this(
                groupId: groupId,
                artifactId: artifactId,
                version: version,
                exclusions: null,
                type: null,
                scope: null,
                optional: false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload")]
        public JavaDependency(
            string groupId,
            string artifactId,
            string version,
            string type = null,
            JavaDependencyScope? scope = null)
            : this(
                groupId: groupId,
                artifactId: artifactId,
                version: version,
                exclusions: null,
                type: type,
                scope: scope,
                optional: false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JavaDependency"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload")]
        public JavaDependency(string groupId, string artifactId, bool optional)
            : this(
                groupId: groupId,
                artifactId: artifactId,
                version: null,
                exclusions: null,
                type: null,
                scope: null,
                optional: optional)
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
        /// The exclusions of the dependency.
        /// </summary>
        public List<JavaDependencyExclusion> Exclusions { get; set; }

        /// <summary>
        /// The type of the dependency.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The scope of the dependency.
        /// </summary>
        public JavaDependencyScope? Scope { get; set; }

        /// <summary>
        /// Whether or not the dependency is optional.
        /// </summary>
        public bool Optional { get; set; }
    }
}