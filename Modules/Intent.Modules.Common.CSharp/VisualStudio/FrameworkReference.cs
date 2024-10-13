using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.VisualStudio
{
    public class FrameworkReference : IAssemblyReference
    {
        public FrameworkReference(string frameworkName)
        {
            Library = frameworkName;
        }

        public bool Equals(IAssemblyReference other)
        {
            if (other == null)
                return false;
            return Library == other.Library;
        }

        public string Library { get; }
        public string HintPath { get; }
        public bool HasHintPath()
        {
            return false;
        }
    }
}
