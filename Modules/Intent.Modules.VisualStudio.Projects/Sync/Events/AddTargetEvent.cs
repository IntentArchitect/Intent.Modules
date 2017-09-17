using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.VisualStudio.Projects.Sync.Events
{
    public class AddTargetEvent 
    {
        public IProject Project { get; }
        public string Name { get; }
        public string Condition { get; }
        public string Xml { get; }

        public AddTargetEvent(IProject project, string name, string condition, string xml)
        {
            Project = project;
            Name = name;
            Condition = condition;
            Xml = xml;
        }

    }
}
