namespace Intent.Modules.VisualStudio.Projects.Events
{
    public class VisualStudioProjectCreatedEvent
    {
        public string ProjectId { get; }
        public string ProjectPath { get; }

        public VisualStudioProjectCreatedEvent(string projectId, string projectPath)
        {
            ProjectId = projectId;
            ProjectPath = projectPath;
        }
    }
}