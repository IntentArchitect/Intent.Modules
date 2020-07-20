using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes
{
    internal interface INuGetSchemeProcessor
    {
        Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode);

        string InstallPackages(
            string projectContent,
            Dictionary<string, NuGetPackage> requestedPackages,
            Dictionary<string, NuGetPackage> installedPackages,
            string projectName,
            ITracing tracing);
    }
}