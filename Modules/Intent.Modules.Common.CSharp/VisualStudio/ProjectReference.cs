using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.VisualStudio
{
    public class ProjectReference : IAssemblyReference
    {
        public string Library { get; }
        public string HintPath { get; }

        public ProjectReference(string projectPath, string hintPath = null)
        {
            Library = projectPath;
            HintPath = hintPath;
        }

        public bool HasHintPath()
        {
            return true;
        }

        public bool Equals(IAssemblyReference other)
        {
            if (other == null)
                return false;
            return Library == other.Library;
        }
    }
}
