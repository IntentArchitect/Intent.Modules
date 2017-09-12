using Intent.SoftwareFactory.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.VSProjectSync.Events
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
