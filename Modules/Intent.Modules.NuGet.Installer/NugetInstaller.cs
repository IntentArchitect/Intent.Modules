using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.NuGet.Installer.Managers;
using Intent.Modules.Common.Plugins;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.NuGet.Installer
{
    public class NugetInstaller : FactoryExtensionBase, IExecutionLifeCycle
    {
        public const string TracingOutputPrefix = "NuGet - ";
        public override string Id => Identifier;
        public const string Identifier = "Intent.NugetInstaller";
        private const string SettingKeyForPerformCleanup = "Perform NuGet Package Cleanup"; // Must match the config entry in the .imodspec
        private const string SettingKeyForAllowPrereleaseVersions = "Allow Prerelease Versions"; // Must match the config entry in the .imodspec
        private const string SettingKeyForConsolidatePackageVersions = "Consolidate Package Versions"; // Must match the config entry in the .imodspec
        private const string SettingKeyForWarnOnMultipleVersionsOfSamePackage = "Warn On Multiple Versions of Same Package"; // Must match the config entry in the .imodspec
        private bool _settingPerformCleanup;
        private bool _settingAllowPrereleaseVersions;
        private bool _settingConsolidatePackageVersions;
        private bool _settingWarnOnMultipleVersionsOfSamePackage;

        public NugetInstaller()
        {
            Order = 100;
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            settings.SetIfSupplied(
                name: SettingKeyForPerformCleanup,
                setSetting: parsedValue => _settingPerformCleanup = parsedValue,
                convert: rawValue => true.ToString().Equals(rawValue, StringComparison.OrdinalIgnoreCase));
            settings.SetIfSupplied(
                name: SettingKeyForAllowPrereleaseVersions,
                setSetting: parsedValue => _settingAllowPrereleaseVersions = parsedValue,
                convert: rawValue => true.ToString().Equals(rawValue, StringComparison.OrdinalIgnoreCase));
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
                Run(application, Logging.Log);
            }
        }

        public void Run(IApplication application, ITracing tracing)
        {
            tracing.Info($"{TracingOutputPrefix}Start processing packages");

            foreach (var project in application.Projects.Where(x => x.ProjectFile() == null))
            {
                tracing.Debug($"{TracingOutputPrefix}Skipped processing project '{project.Name}' as its type '{project.ProjectType.Name}' is unsupported.");
            }

            var netFrameworkProjects = application.Projects
                .Where(x => x.ProjectFile() != null && !IsNetCoreProject(x))
                .ToArray();

            if (netFrameworkProjects.Any())
            {
                InstallForNetFramework(application.GetSolutionPath(), netFrameworkProjects, tracing);
            }

            var netCoreProjects = application.Projects
                .Where(x => x.ProjectFile() != null && IsNetCoreProject(x))
                .ToArray();

            InstallForNetCore(netCoreProjects, tracing);

            tracing.Info($"{TracingOutputPrefix}Completed processing packages");
        }

        private void InstallForNetFramework(string solutionPath, IEnumerable<IProject> applicableProjects, ITracing tracing)
        {
            var addFileBehaviours = applicableProjects
                .SelectMany(x => x.NugetPackages())
                .GroupBy(x => x.Name)
                .Distinct()
                .ToDictionary(x => x.Key, x => new CanAddFileStrategy(x) as ICanAddFileStrategy);

            using (var nugetManager = new NugetManager(
                solutionFilePath: solutionPath,
                tracing: tracing,
                settings: new NuGetManagerSettings
                {
                    AllowPreReleaseVersions = _settingAllowPrereleaseVersions,
                    ConsolidateVersions = _settingConsolidatePackageVersions,
                    WarnOnMultipleVersionsOfSamePackage = _settingWarnOnMultipleVersionsOfSamePackage
                },
                projectFilePaths: applicableProjects.Select(x => Path.GetFullPath(x.ProjectFile())),
                canAddFileStrategies: addFileBehaviours))
            {
                foreach (var project in applicableProjects)
                {
                    var projectFile = Path.GetFullPath(project.ProjectFile());

                    tracing.Info($"{TracingOutputPrefix}Determining Packages for installation - {project.ProjectFile()}");

                    var nugetPackages = project.NugetPackages();
                    foreach (var nugetPackageInfo in nugetPackages)
                    {
                        nugetManager.AddNugetPackage(
                            projectFile: projectFile,
                            packageId: nugetPackageInfo.Name,
                            minVersionInclusive: nugetPackageInfo.Version);
                    }
                }

                tracing.Info($"{TracingOutputPrefix}Processing pending installs...");
                nugetManager.ProcessPendingInstalls();

                if (_settingPerformCleanup)
                {
                    tracing.Info($"{TracingOutputPrefix}Cleaning up packages folders...");
                    nugetManager.CleanupPackagesFolder();
                }
            }
        }

        private static void InstallForNetCore(IProject[] netCoreProjects, ITracing tracing)
        {
            foreach (var netCoreProject in netCoreProjects)
            {
                var doc = XDocument.Load(netCoreProject.ProjectFile());

                var nugetPackages = netCoreProject
                    .NugetPackages()
                    .Distinct()
                    .GroupBy(x => x.Name)
                    .ToDictionary(x => x.Key, x => x);

                var packageReferenceItemGroup = doc.XPathSelectElement("Project/ItemGroup[PackageReference]");
                if (packageReferenceItemGroup == null)
                {
                    packageReferenceItemGroup = new XElement("ItemGroup");
                    doc.XPathSelectElement("Project").Add(packageReferenceItemGroup);
                }

                foreach (var addFileBehaviour in nugetPackages)
                {
                    var latestVersion = addFileBehaviour.Value.OrderByDescending(x => x.Version).First().Version;
                    var existingReference = packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{addFileBehaviour.Key}']");

                    if (existingReference == null)
                    {
                        tracing.Info($"{TracingOutputPrefix}Installing {addFileBehaviour.Key} {latestVersion} into project {netCoreProject.Name}");

                        packageReferenceItemGroup.Add(new XElement("PackageReference",
                            new XAttribute("Include", addFileBehaviour.Key),
                            new XAttribute("Version", latestVersion)));
                    }
                }

                doc.Save(netCoreProject.ProjectFile());
            }
        }

        private bool IsNetCoreProject(IProject project)
        {
            var doc = XDocument.Load(Path.GetFullPath(project.ProjectFile()));
            return doc.XPathSelectElement("Project[@Sdk]") != null;
        }

        private class CanAddFileStrategy : ICanAddFileStrategy
        {
            private readonly INugetPackageInfo[] _nugetPackageInfos;

            public CanAddFileStrategy(IEnumerable<INugetPackageInfo> nugetPackageInfos)
            {
                _nugetPackageInfos = nugetPackageInfos.ToArray();

                if (_nugetPackageInfos.Any(x => _nugetPackageInfos.All(y => !y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase))))
                {
                    throw new Exception("All package ids should be the same.");
                }
            }

            public bool CanAddFile(string file)
            {
                return _nugetPackageInfos.All(x => x.CanAddFile(file));
            }
        }
    }
}