using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.VisualStudio.Projects.Sync.Events
{
    public class RemoveProjectItemEvent
    {
        public IProject Project { get; }
        public string RelativeFileName { get; }

        public RemoveProjectItemEvent(IProject project, string relativeFileName)
        {
            Project = project;
            RelativeFileName = relativeFileName;
        }
    }
}
