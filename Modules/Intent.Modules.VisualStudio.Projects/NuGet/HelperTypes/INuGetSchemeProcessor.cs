using System.Collections.Generic;
using System.Xml.Linq;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes
{
    internal interface INuGetSchemeProcessor
    {
        Dictionary<string, NuGetPackage> GetInstalledPackages(IProject project, XNode xNode);

        string InstallPackages(
            string projectContent,
            Dictionary<string, NuGetPackage> requestedPackages,
            Dictionary<string, NuGetPackage> installedPackages,
            string projectName,
            ITracing tracing);
    }
}