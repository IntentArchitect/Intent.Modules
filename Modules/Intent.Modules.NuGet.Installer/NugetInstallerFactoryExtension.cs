using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Schemes;
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
        private readonly IDictionary<ProjectType, INuGetScheme> NuGetProjectSchemes;

        public NugetInstallerFactoryExtension()
        {
            NuGetProjectSchemes = new Dictionary<ProjectType, INuGetScheme>
            {
                { ProjectType.Lean, new LeanScheme() },
                { ProjectType.Unsupported, new UnsupportedScheme() },
                { ProjectType.VerboseWithPackageReferenceScheme, new VerboseWithPackageReferencesScheme() },
                { ProjectType.VerboseWithPackagesDotConfigScheme, new VerboseWithPackagesDotConfigScheme() }
            };
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
                ResolveScheme(projectPackage.Type).InstallPackages(projectPackage, tracing);
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

        private INuGetScheme ResolveScheme(ProjectType projectType)
        {
            if (!NuGetProjectSchemes.TryGetValue(projectType, out var scheme))
            {
                throw new ArgumentOutOfRangeException(nameof(projectType), $"No scheme registered for type {projectType}.");
            }

            return scheme;
        }

        private (IReadOnlyCollection<NuGetProject> Projects, Dictionary<string, SemanticVersion> HighestVersions) DeterminePackages(IApplication application)
        {
            var projects = new List<NuGetProject>();
            var highestVersions = new Dictionary<string, SemanticVersion>();

            foreach (var project in application.Projects.OrderBy(x => x.Name))
            {
                var document = project.ProjectFile() != null
                    ? XDocument.Load(Path.GetFullPath(project.ProjectFile()))
                    : null;

                var projectType = GetProjectType(document);


                var installedPackages = ResolveScheme(projectType).GetInstalledPackages(project, document);

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

        private static ProjectType GetProjectType(XNode xNode)
        {
            if (xNode == null)
            {
                return ProjectType.Unsupported;
            }

            if (xNode.XPathSelectElement("Project[@Sdk]") != null)
            {
                return ProjectType.Lean;
            }

            if (xNode.XPathSelectElement("/ns:Project/ns:ItemGroup/ns:None[@Include='packages.config']") != null)
            {
                return ProjectType.VerboseWithPackagesDotConfigScheme;
            }

            // Even if there is no PackageReference element, so long as there is no packages.config, then we are free to to use
            // PackageReferences going forward. In the event there is PackageReference element, then we are of course already
            // using the PackageReference scheme.
            return ProjectType.VerboseWithPackageReferenceScheme;
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
    }
}
