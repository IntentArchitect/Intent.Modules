namespace Intent.Modules.Common.Java.Templates
{
    public class JavaDependency
    {
        public JavaDependency(string groupId, string artifactId, string version = null)
        {
            GroupId = groupId;
            ArtifactId = artifactId;
            Version = version;
        }
        public string GroupId { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
    }
}