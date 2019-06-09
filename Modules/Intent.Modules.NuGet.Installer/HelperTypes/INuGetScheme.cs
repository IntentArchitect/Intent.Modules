using System.Collections.Generic;
using System.Xml.Linq;
using Intent.SoftwareFactory.Engine;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.HelperTypes
{
    internal interface INuGetScheme
    {
        Dictionary<string, SemanticVersion> GetInstalledPackages(IProject project, XNode xNode);
        void InstallPackages(NuGetProject project, ITracing tracing);
    }
}