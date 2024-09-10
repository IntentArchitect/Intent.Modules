using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.VisualStudio
{
    public interface INugetPackageDependency
    {
        string Name { get; }
        string Version { get; }
    }
}
