using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.VisualStudio
{
    public class ExternalAssemblyRefernce : IAssemblyReference
    {
        public string Library { get; }
        public string HintPath { get; }

        public ExternalAssemblyRefernce(string library, string hintPath)
        {
            Library = library;
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
