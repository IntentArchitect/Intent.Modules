using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.NuGet.Installer.SchemeProcessors
{
    internal class UnsupportedSchemeProcessor : INuGetSchemeProcessor
    {
        public Dictionary<string, NuGetPackage> GetInstalledPackages(IProject project, XNode xNode)
        {
            return new Dictionary<string, NuGetPackage>();
        }

        public string InstallPackages(
            string projectContent,
            Dictionary<string, NuGetPackage> requestedPackages,
            Dictionary<string, NuGetPackage> installedPackages,
            string projectName,
            ITracing tracing)
        {
            tracing.Debug($"Skipped processing project '{projectName}' as its type is unsupported.");

            return projectContent;
        }
    }
}
