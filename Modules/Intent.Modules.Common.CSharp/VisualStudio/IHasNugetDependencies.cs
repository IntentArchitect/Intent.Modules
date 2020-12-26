using System.Collections.Generic;

namespace Intent.Modules.Common.VisualStudio
{
    public interface IHasNugetDependencies
    {
        IEnumerable<INugetPackageInfo> GetNugetDependencies();
    }
}
