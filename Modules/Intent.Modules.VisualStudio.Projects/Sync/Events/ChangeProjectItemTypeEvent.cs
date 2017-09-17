using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.VisualStudio.Projects.Sync.Events
{
    public class ChangeProjectItemTypeEvent
    {
        public IProject Project { get; }
        public string RelativeFileName { get; }
        public string ItemType { get; }

        public ChangeProjectItemTypeEvent(IProject project, string relativeFileName, string itemType)
        {
            Project = project;
            RelativeFileName = relativeFileName;
            ItemType = itemType;
        }
    }
}
