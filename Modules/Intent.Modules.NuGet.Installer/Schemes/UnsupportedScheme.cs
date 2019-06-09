using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Engine;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.Schemes
{
    internal class UnsupportedScheme : INuGetScheme
    {
        public Dictionary<string, SemanticVersion> GetInstalledPackages(IProject project, XNode xNode)
        {
            return new Dictionary<string, SemanticVersion>();
        }

        public void InstallPackages(NuGetProject project, ITracing tracing)
        {
            tracing.Debug($"Skipped processing project '{project.Project.Name}' as its type '{project.Project.ProjectType.Name}' is unsupported.");
        }
    }
}
