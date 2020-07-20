using System.Collections.Generic;
using Intent.Configuration;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public class ProjectOutput : IOutputTargetRole
    {
        public ProjectOutput(string id, string relativeLocation)
        {
            Id = id;
            RelativeLocation = relativeLocation;
        }
        public string Id { get; }
        public string RelativeLocation { get; }
        public IEnumerable<string> RequiredFrameworks { get; }
    }
}