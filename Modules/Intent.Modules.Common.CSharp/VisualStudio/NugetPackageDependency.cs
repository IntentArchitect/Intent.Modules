using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.VisualStudio
{
    public class NugetPackageDependency : INugetPackageDependency
    {
        public NugetPackageDependency(string name, string version)
        {
            Name = name; 
            Version = version;
        }

        public override string ToString()
        {
            return $"{Name}, {Version}";
        }

        public string Name { get; }
        public string Version { get; }
    }
}
