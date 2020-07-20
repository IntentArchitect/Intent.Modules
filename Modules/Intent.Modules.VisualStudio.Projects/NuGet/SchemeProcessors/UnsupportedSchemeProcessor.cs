using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;

namespace Intent.Modules.VisualStudio.Projects.NuGet.SchemeProcessors
{
    internal class UnsupportedSchemeProcessor : INuGetSchemeProcessor
    {
        public Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode)
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
