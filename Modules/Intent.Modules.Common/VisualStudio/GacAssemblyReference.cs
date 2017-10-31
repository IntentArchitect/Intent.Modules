using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.VisualStudio
{
    public class GacAssemblyReference : IAssemblyReference
    {
        public GacAssemblyReference(string library)
        {
            Library = library;
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
