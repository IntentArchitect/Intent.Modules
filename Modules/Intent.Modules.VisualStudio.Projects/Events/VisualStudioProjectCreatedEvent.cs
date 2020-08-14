using Intent.Modules.VisualStudio.Projects.Templates;

namespace Intent.Modules.VisualStudio.Projects.Events
{
    public class VisualStudioProjectCreatedEvent
    {
        public string ProjectId { get; }
        public IVisualStudioProjectTemplate TemplateInstance { get; }

        public VisualStudioProjectCreatedEvent(string projectId, IVisualStudioProjectTemplate templateInstance)
        {
            ProjectId = projectId;
            TemplateInstance = templateInstance;
        }
    }
}