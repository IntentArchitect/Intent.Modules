using Intent.SoftwareFactory.Engine;

namespace Intent.SoftwareFactory.VSProjectSync.Events
{
    public class AddTaskEvent 
    {
        public IProject Project { get; }
        public string TaskName { get; }
        public string AssemblyFile { get; }

        public AddTaskEvent(IProject project, string taskName, string assemblyFile)
        {
            Project = project;
            TaskName = taskName;
            AssemblyFile = assemblyFile;
        }
    }
}
