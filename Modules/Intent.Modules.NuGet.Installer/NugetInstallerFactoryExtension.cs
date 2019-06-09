using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer
{
    public class NugetInstallerFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => Identifier;
        public const string Identifier = "Intent.NugetInstaller";
        private const string SettingKeyForConsolidatePackageVersions = "Consolidate Package Versions"; // Must match the config entry in the .imodspec
        private const string SettingKeyForWarnOnMultipleVersionsOfSamePackage = "Warn On Multiple Versions of Same Package"; // Must match the config entry in the .imodspec
        private bool _settingConsolidatePackageVersions;
        private bool _settingWarnOnMultipleVersionsOfSamePackage;

        private class TracingWithPrefix : ITracing
        {
            private readonly ITracing _tracing;
            private const string TracingOutputPrefix = "NuGet - ";

            public TracingWithPrefix(ITracing tracing) => _tracing = tracing;

            public void Debug(string message) => _tracing.Debug($"{TracingOutputPrefix}{message}");

            public void Failure(Exception exception) => _tracing.Failure(exception);

            public void Failure(string exceptionMessage) => _tracing.Failure($"{TracingOutputPrefix}{exceptionMessage}");

            public void Info(string message) => _tracing.Info($"{TracingOutputPrefix}{message}");

            public void Warning(string message) => _tracing.Warning($"{TracingOutputPrefix}{message}");
        }

        public override int Order { get; set; } = 100;

        public override void Configure(IDictionary<string, string> settings)
        {
            settings.SetIfSupplied(
                name: SettingKeyForConsolidatePackageVersions,
                setSetting: parsedValue => _settingConsolidatePackageVersions = parsedValue,
                convert: rawValue => true.ToString().Equals(rawValue, StringComparison.OrdinalIgnoreCase));
            settings.SetIfSupplied(
                name: SettingKeyForWarnOnMultipleVersionsOfSamePackage,
                setSetting: parsedValue => _settingWarnOnMultipleVersionsOfSamePackage = parsedValue,
                convert: rawValue => true.ToString().Equals(rawValue, StringComparison.OrdinalIgnoreCase));

            base.Configure(settings);
        }

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                Run(application, new TracingWithPrefix(Logging.Log));
            }
        }

        public void Run(IApplication application, ITracing tracing)
        {
            string report;

            tracing.Info("Start processing packages");

            var (projectPackages, highestVersions) = DeterminePackages(application);
            if (_settingConsolidatePackageVersions &&
                !string.IsNullOrWhiteSpace(report = GetPackagesWithMultipleVersionsReport(projectPackages)))
            {
                tracing.Info(
                    "Multiple versions exist for one or more NuGet packages within the solution. Intent will now automatically " +
                           "upgrade any lower versions to the highest installed version within the solution. To disable this behaviour " +
                           $"change the 'Consolidate Package Versions' option in Intent in the {Identifier} module configuration." +
                           $"{Environment.NewLine}" +
                           $"{Environment.NewLine}" +
                           $"{report}");

                ConsolidatePackageVersions(projectPackages, highestVersions);
            }

            foreach (var projectPackage in projectPackages)
            {
                switch (projectPackage.Type)
                {
                    case ProjectType.Lean:
                        InstallForLeanScheme(projectPackage, tracing);
                        break;
                    case ProjectType.VerboseWithPackageReferenceScheme:
                        InstallForVerboseWithPackageReferenceScheme(projectPackage, tracing);
                        break;
                    case ProjectType.VerboseWithPackagesDotConfigScheme:
                        InstallForVerboseWithPackagesDotConfigScheme(projectPackage, tracing);
                        break;
                    case ProjectType.Unsupported:
                        tracing.Debug($"Skipped processing project '{projectPackage.Project.Name}' as its type '{projectPackage.Project.ProjectType.Name}' is unsupported.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                projectPackage.Document?.Save(projectPackage.Project.ProjectFile());
            }

            if (_settingWarnOnMultipleVersionsOfSamePackage &&
                !_settingConsolidatePackageVersions &&
                !string.IsNullOrWhiteSpace(report = GetPackagesWithMultipleVersionsReport(projectPackages)))
            {
                tracing.Warning(
                    "Multiple versions exist for one or more NuGet packages within the solution. You should consider " +
                           "consolidating these package versions within Visual Studio or alternatively enable the 'Consolidate " +
                           $"Package Versions' option in Intent in the {Identifier} module configuration." +
                           $"{Environment.NewLine}" +
                           $"{Environment.NewLine}" +
                           $"{report}");
            }

            tracing.Info("Package processing complete");
        }

        private static (IReadOnlyCollection<NuGetProject> Projects, Dictionary<string, SemanticVersion> HighestVersions) DeterminePackages(IApplication application)
        {
            var projects = new List<NuGetProject>();
            var highestVersions = new Dictionary<string, SemanticVersion>();

            foreach (var project in application.Projects.OrderBy(x => x.Name))
            {
                var document = project.ProjectFile() != null
                    ? XDocument.Load(Path.GetFullPath(project.ProjectFile()))
                    : null;

                var projectType = GetProjectType(document);

                Dictionary<string, SemanticVersion> installedPackages;
                switch (projectType)
                {
                    case ProjectType.Lean:
                        installedPackages = GetInstalledPackagesForLean(document);
                        break;
                    case ProjectType.VerboseWithPackageReferenceScheme:
                        installedPackages = GetInstalledPackagesForVerboseWithPackageReferenceScheme(document);
                        break;
                    case ProjectType.VerboseWithPackagesDotConfigScheme:
                        installedPackages = GetPackagesDotConfigEntries(project);
                        break;
                    case ProjectType.Unsupported:
                        installedPackages = new Dictionary<string, SemanticVersion>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(projectType), projectType, null);
                }

                var requestedPackages = project
                    .NugetPackages()
                    .GroupBy(
                        keySelector: x => x.Name,
                        elementSelector: x =>
                        {
                            if (!SemanticVersion.TryParse(x.Version, out var semanticVersion))
                            {
                                throw new Exception($"Could not parse '{x.Version}' from Intent metadata for package '{x.Name}' in project '{project.Name}' as a valid Semantic Version 2.0 'version' value.");
                            }

                            return semanticVersion;
                        })
                    .ToDictionary(
                        keySelector: x => x.Key,
                        elementSelector: x =>
                        {
                            var requestedVersion = x.Max();

                            if (!highestVersions.ContainsKey(x.Key))
                            {
                                highestVersions.Add(x.Key, requestedVersion);
                            }
                            else if (highestVersions[x.Key] < requestedVersion)
                            {
                                highestVersions[x.Key] = requestedVersion;
                            }

                            return requestedVersion;
                        });

                projects.Add(new NuGetProject
                {
                    Document = document,
                    Project = project,
                    Type = projectType,
                    RequestedPackages = requestedPackages,
                    InstalledPackages = installedPackages
                });
            }

            return (projects, highestVersions);
        }

        private static ProjectType GetProjectType(XNode doc)
        {
            if (doc == null)
            {
                return ProjectType.Unsupported;
            }

            if (doc.XPathSelectElement("Project[@Sdk]") != null)
            {
                return ProjectType.Lean;
            }

            if (doc.XPathSelectElement("/ns:Project/ns:ItemGroup/ns:None[@Include='packages.config']") != null)
            {
                return ProjectType.VerboseWithPackagesDotConfigScheme;
            }

            // Event if there is no PackageReference element, so long as there is no packages.config, then we are free to to use
            // PackageReferences going forward. In the event there is PackageReference element, then we are of course already
            // using the PackageReference scheme.
            return ProjectType.VerboseWithPackageReferenceScheme;
        }

        private static Dictionary<string, SemanticVersion> GetInstalledPackagesForLean(XNode xNode)
        {
            var packageReferenceElements = xNode.XPathSelectElements("Project/ItemGroup/PackageReference");
            return packageReferenceElements.ToDictionary(
                x => x.Attribute("Include")?.Value,
                x => SemanticVersion.Parse(x.Attribute("Version")?.Value));
        }

        private static Dictionary<string, SemanticVersion> GetInstalledPackagesForVerboseWithPackageReferenceScheme(XNode xNode)
        {
            var packageReferenceElements = xNode.XPathSelectElements("Project/ItemGroup/PackageReference");
            return packageReferenceElements.ToDictionary(
                x => x.Attribute("Include")?.Value,
                x => SemanticVersion.Parse(x.Elements().Single(e => e.Name == "Version")?.Value));
        }

        private static void ConsolidatePackageVersions(IReadOnlyCollection<NuGetProject> projectPackages, IDictionary<string, SemanticVersion> highestVersions)
        {
            foreach (var highestVersion in highestVersions)
            {
                foreach (var projectPackage in projectPackages)
                {
                    if (projectPackage.RequestedPackages.TryGetValue(highestVersion.Key, out var requestedVersion) &&
                        requestedVersion < highestVersion.Value)
                    {
                        projectPackage.RequestedPackages[highestVersion.Key] = highestVersion.Value;
                    }
                }
            }
        }

        private static Dictionary<string, SemanticVersion> GetPackagesDotConfigEntries(IProject project)
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

        private static void InstallForLeanScheme(NuGetProject project, ITracing tracing)
        {
            var packageReferenceItemGroup = project.Document.XPathSelectElement("Project/ItemGroup[PackageReference]");
            if (packageReferenceItemGroup == null)
            {
                var projectElement = project.Document.XPathSelectElement("Project");
                if (projectElement == null)
                {
                    throw new Exception("Project element not found.");
                }

                projectElement.Add(packageReferenceItemGroup = new XElement("ItemGroup"));
            }

            foreach (var requestedPackage in project.RequestedPackages)
            {
                var packageId = requestedPackage.Key;
                var packageVersion = requestedPackage.Value;

                var packageReferenceElement = packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{packageId}']");
                if (packageReferenceElement == null)
                {
                    tracing.Info($"Installing {packageId} {packageVersion} into project {project.Project.Name}");

                    packageReferenceItemGroup.Add(new XElement("PackageReference",
                        new XAttribute("Include", packageId),
                        new XAttribute("Version", packageVersion)));
                }
            }
        }

        private static void InstallForVerboseWithPackageReferenceScheme(NuGetProject project, ITracing tracing)
        {
            var packageReferenceItemGroup = project.Document.XPathSelectElement("Project/ItemGroup[PackageReference]");
            if (packageReferenceItemGroup == null)
            {
                var projectElement = project.Document.XPathSelectElement("Project");
                if (projectElement == null)
                {
                    throw new Exception("Project element not found.");
                }

                projectElement.Add(packageReferenceItemGroup = new XElement("ItemGroup"));
            }

            foreach (var requestedPackage in project.RequestedPackages)
            {
                var packageId = requestedPackage.Key;
                var packageVersion = requestedPackage.Value;

                var packageReferenceElement = packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{packageId}']");
                if (packageReferenceElement == null)
                {
                    tracing.Info($"Installing {packageId} {packageVersion} into project {project.Project.Name}");

                    packageReferenceItemGroup.Add(new XElement("PackageReference",
                        new XAttribute("Include", packageId),
                        new XElement("Version", packageVersion)));
                }
            }
        }

        private static void InstallForVerboseWithPackagesDotConfigScheme(NuGetProject project, ITracing tracing)
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

        private static string GetPackagesWithMultipleVersionsReport(IEnumerable<NuGetProject> nuGetProjects)
        {
            var resolvedProjects = nuGetProjects
                .Select(x => new
                {
                    ConsolidatedPackageVersions = x.GetConsolidatedPackages(),
                    ProjectName = x.Project.Name
                })
                .ToArray();

            var packagesWithMultipleVersions = resolvedProjects
                .SelectMany(x => x.ConsolidatedPackageVersions)
                .GroupBy(x => x.Key, x => x.Value)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToArray();

            if (!packagesWithMultipleVersions.Any())
            {
                return null;
            }

            return packagesWithMultipleVersions
                .Select(packageId => new
                {
                    PackageId = packageId,
                    VersionReport = resolvedProjects
                        .Where(x => x.ConsolidatedPackageVersions.ContainsKey(packageId))
                        .Select(x => new
                        {
                            x.ProjectName,
                            Version = x.ConsolidatedPackageVersions[packageId]
                        })
                        .OrderByDescending(x => x.Version)
                        .ThenBy(x => x.ProjectName)
                        .Select(x => $"    Version {x.Version} in '{x.ProjectName}'")
                        .Aggregate((x, y) => x + Environment.NewLine + y)
                })
                .Select(x => $"{x.PackageId} has the following versions installed:{Environment.NewLine}{x.VersionReport}")
                .Aggregate((x, y) => x + Environment.NewLine + y);
        }

        private enum ProjectType
        {
            /// <summary>
            /// New lean format, required for use by .NET Standard and .NET Core project types.
            /// </summary>
            Lean,

            /// <summary>
            /// The old verbose format used by .NET Framework projects, set to use newer PackageReference NuGet scheme.
            /// </summary>
            VerboseWithPackageReferenceScheme,

            /// <summary>
            /// The old verbose format used by .NET Framework projects, set to use older packages.config NuGet scheme.
            /// </summary>
            VerboseWithPackagesDotConfigScheme,

            /// <summary>
            /// Unsupported / unknown projected type.
            /// </summary>
            Unsupported
        }

        private class NuGetProject
        {
            public IProject Project { get; set; }
            public ProjectType Type { get; set; }
            public XDocument Document { get; set; }
            public Dictionary<string, SemanticVersion> RequestedPackages { get; set; } = new Dictionary<string, SemanticVersion>();
            public Dictionary<string, SemanticVersion> InstalledPackages { get; set; } = new Dictionary<string, SemanticVersion>();
            public Dictionary<string, SemanticVersion> GetConsolidatedPackages()
            {
                var allPackages = RequestedPackages.ToList();
                allPackages.AddRange(InstalledPackages);

                return allPackages
                    .GroupBy(x => x.Key, x => x.Value)
                    .ToDictionary(x => x.Key, x => x.Max());
            }
        }
    }
}