using System.Collections.Generic;
using System.Xml.Linq;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.NuGet.Installer.HelperTypes
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