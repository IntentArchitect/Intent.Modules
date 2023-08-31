namespace Intent.Modules.Common.Java.Templates;

/// <summary>
/// Options for <see cref="JavaDependency.Scope"/>. See <see href="https://maven.apache.org/pom.html#exclusions"/> for more details.
/// </summary>
public class JavaDependencyExclusion
{
    /// <summary>
    /// Creates a new instance of <see cref="JavaDependencyExclusion"/>.
    /// </summary>
    public JavaDependencyExclusion(string groupId, string artifactId)
    {
        GroupId = groupId;
        ArtifactId = artifactId;
    }

    /// <summary>
    /// The <c>groupId</c> of the exclusion.
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// The <c>artifactId</c> of the exclusion.
    /// </summary>
    public string ArtifactId { get; set; }
}