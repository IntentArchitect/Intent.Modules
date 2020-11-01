using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.NuGet.SchemeProcessors
{
    internal class VerboseWithPackagesDotConfigSchemeProcessor : INuGetSchemeProcessor
    {
        public Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode)
        {
            var projectFileDirectory = Path.GetDirectoryName(projectPath);
            if (projectFileDirectory == null)
            {
                throw new Exception("projectFileDirectory is null.");
            }

            var packagesDotConfigPath = Path.Combine(projectFileDirectory, "packages.config");
            if (!File.Exists(packagesDotConfigPath))
            {
                throw new Exception($"Could not find `{packagesDotConfigPath}`");
            }

            var d = XDocument.Load(packagesDotConfigPath);
            var rootElements = d.Elements().ToArray();
            if (rootElements.Length != 1 || rootElements.Single().Name != "packages")
            {
                throw new Exception($"Error reading '{packagesDotConfigPath}', expected single root element called 'packages'");
            }

            var subElements = rootElements.Single().Elements().ToArray();
            if (subElements.Any(x => x.Name != "package"))
            {
                throw new Exception($"Error reading '{packagesDotConfigPath}', expected all child elements to be called 'package'");
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
                !NuGetVersion.TryParse(x.Version, out _)))
            {
                throw new Exception($"Error reading '{packagesDotConfigPath}', expected all child elements to have both an 'id' and a valid Semantic Version 2.0 'version' value.");
            }

            return nugetPackages.ToDictionary(x => x.Id, x => NuGetPackage.Create(x.Version, new string[0], new string[0]));
        }

        public string InstallPackages(
            string projectContent,
            Dictionary<string, NuGetPackage> requestedPackages,
            Dictionary<string, NuGetPackage> installedPackages,
            string projectName,
            ITracing tracing)
        {
            // This format is now unsupported, but we will show a warnings for missing packages.

            var installationsRequired = requestedPackages
                .Where(x => !installedPackages.ContainsKey(x.Key))
                .ToArray();
            var upgradesRequired = requestedPackages
                .Where(x => installedPackages.TryGetValue(x.Key, out var nuGetPackage) && nuGetPackage.Version.MinVersion < x.Value.Version.MinVersion)
                .ToArray();

            if (!installationsRequired.Any() && !upgradesRequired.Any())
            {
                return projectContent;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Installations and upgrades of NuGet packages is not supported for project {projectName}, only .csproj files using " +
                          "PackageReferences are. Refer to https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html for information " +
                          "on how to upgrade this existing project. You can alternatively manually install/upgrade the following packages using Visual Studio:");
            foreach (var item in installationsRequired)
            {
                sb.AppendLine($"  - Install {item.Key} version {item.Value.Version}");
            }

            foreach (var item in upgradesRequired)
            {
                sb.AppendLine($"  - Upgrade version of {item.Key} from {installedPackages[item.Key]} to {item.Value.Version}");
            }

            tracing.Warning(sb.ToString());

            return projectContent;
        }
    }
}
