using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.VisualStudio
{
    public interface IHasNugetDependencies
    {
        IEnumerable<INugetPackageInfo> GetNugetDependencies();
    }
}
