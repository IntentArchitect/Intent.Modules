using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Engine;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.Schemes
{
    internal class VerboseWithPackagesDotConfigScheme : INuGetScheme
    {
        public Dictionary<string, SemanticVersion> GetInstalledPackages(IProject project, XNode xNode)
        {
            var projectFileDirectory = Path.GetDirectoryName(project.ProjectFile());
            if (projectFileDirectory == null)
            {
                throw new Exception("projectFileDirectory is null.");
            }

            var packagesDotConfigPath = Path.Combine(projectFileDirectory, "packages.config");
            if (!File.Exists(packagesDotConfigPath))
            {
                return null;
            }

            var d = XDocument.Load(packagesDotConfigPath);
            var rootElements = d.Elements().ToArray();
            if (rootElements.Length != 1 || rootElements.Single().Name != "Packages")
            {
                throw new Exception($"Error reading '{packagesDotConfigPath}', expected single root element called 'Packages'");
            }

            var subElements = rootElements.Single().Elements().ToArray();
            if (subElements.Any(x => x.Name != "Package"))
            {
                throw new Exception($"Error reading '{packagesDotConfigPath}', expected all child elements to be called 'Package'");
            }

            var nugetPackages = subElements
                .Select(x => new
                {
                    Id = x.Attribute("id")?.Value,
                    Version = x.Attribute("version")?.Value
                })
                .ToArray();
            if (nugetPackages.Any(x =>
                string.IsNullOrWhiteSpace(x.Id) ||
                string.IsNullOrWhiteSpace(x.Version) ||
                !SemanticVersion.TryParse(x.Version, out _)))
            {
                throw new Exception($"Error reading '{packagesDotConfigPath}', expected all child elements to have both an 'id' and a valid Semantic Version 2.0 'version' value.");
            }

            return nugetPackages.ToDictionary(x => x.Id, x => SemanticVersion.Parse(x.Version));
        }

        public void InstallPackages(NuGetProject project, ITracing tracing)
        {
            // This format is now unsupported, but we will show a warning on it missing a requested NuGet package.

            var installationsRequired = project.RequestedPackages
                .Where(x => !project.InstalledPackages.ContainsKey(x.Key))
                .ToArray();
            var upgradesRequired = project.RequestedPackages
                .Where(x => project.InstalledPackages.TryGetValue(x.Key, out var version) && version < x.Value)
                .ToArray();

            if (!installationsRequired.Any() && !upgradesRequired.Any())
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Installations and upgrades of NuGet packages is not supported for project {project.Project.Name}, only .csproj files using " +
                          "PackageReferences are. Refer to https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html for information " +
                          "on how to upgrade this existing project. You can alternatively manually install/upgrade the following packages using Visual Studio:");
            foreach (var item in installationsRequired)
            {
                sb.AppendLine($"  - Install {item.Key} version {item.Value}");
            }

            foreach (var item in upgradesRequired)
            {
                sb.AppendLine($"  - Upgrade version of {item.Key} from {project.InstalledPackages[item.Key]} to {item.Value}");
            }

            tracing.Warning(sb.ToString());
        }
    }
}
