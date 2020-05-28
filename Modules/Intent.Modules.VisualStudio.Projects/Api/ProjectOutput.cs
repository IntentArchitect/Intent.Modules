using Intent.Configuration;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public class ProjectOutput : IProjectOutputTarget
    {
        public ProjectOutput(string id, string relativeLocation)
        {
            Id = id;
            RelativeLocation = relativeLocation;
        }
        public string Id { get; }
        public string RelativeLocation { get; }
    }
}