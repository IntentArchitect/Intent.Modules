using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.NuGet.Installer.NugetIntegration;
using Intent.SoftwareFactory.Engine;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NuGet;
using IPackage = NuGet.IPackage;

namespace Intent.Modules.NuGet.Installer.Managers
{
    public class NugetManager : IDisposable
    {
        private readonly ITracing _tracing;
        private readonly NuGetManagerSettings _settings;
        private readonly INugetServices _nugetServices;
        private readonly List<MsbuildProject> _msbuildProjects = new List<MsbuildProject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solutionFilePath"></param>
        /// <param name="tracing"></param>
        /// <param name="settings"></param>
        /// <param name="projectFilePaths">Project files which may not be saved to the .sln file yet, but should be processed as well.</param>
        /// <param name="canAddFileStrategies">Strategy to control whether or not NuGet packages can add source files to projects. For example Startup.cs by Owin.</param>
        public NugetManager(
            string solutionFilePath,
            ITracing tracing,
            NuGetManagerSettings settings,
            IEnumerable<string> projectFilePaths = null,
            IDictionary<string, ICanAddFileStrategy> canAddFileStrategies = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (solutionFilePath == null)
                throw new ArgumentNullException(nameof(solutionFilePath));

            if (!File.Exists(solutionFilePath))
                throw new ArgumentException($"File '{solutionFilePath}' not found.", nameof(solutionFilePath));

            if (projectFilePaths == null)
                projectFilePaths = new string[0];

            _tracing = tracing;
            _settings = settings;
            _nugetServices = NugetServices.Create(solutionFilePath, canAddFileStrategies, tracing);

            LoadMsbuildProjects(solutionFilePath, projectFilePaths);
        }

        public void AddNugetPackage(string projectFile, string packageId, string minVersionInclusive)
        {
            AddOrUpdateRequiredPackage(
                msbuildProject: GetMsbuildProject(projectFile),
                packageId: packageId,
                referer: Referer.Create("Software Factory", Utility.GetMinVersionSpecInclusive(minVersionInclusive)));
        }

        public void CleanupPackagesFolder()
        {
            _nugetServices.CleanupPackagesFolder();
        }

        public void ProcessPendingInstalls()
        {
            string report;
            if (_settings.ConsolidateVersions && !string.IsNullOrWhiteSpace(report = GetPackagesWithMultipleVersionsReport()))
            {
                _tracing.Info($"{NugetInstaller.TracingOutputPrefix}" +
                                $"Multiple versions exist for one or more NuGet packages within the solution. Intent will now automatically " +
                                $"upgrade any lower versions to the highest installed version within the solution. To disable this behaviour " +
                                $"change the 'Consolidate Package Versions' option in Intent in the {NugetInstaller.Identifier} module " +
                                $"configuration." +
                                $"{Environment.NewLine}" +
                                $"{Environment.NewLine}" +
                                $"{report}");

                foreach (var msbuildProject in _msbuildProjects)
                {
                    ConsolidateVersions(msbuildProject);
                }
            }
            else if (!_settings.ConsolidateVersions && _settings.WarnOnMultipleVersionsOfSamePackage && !string.IsNullOrWhiteSpace(report = GetPackagesWithMultipleVersionsReport()))
            {
                _tracing.Warning($"{NugetInstaller.TracingOutputPrefix}" +
                                 $"Multiple versions exist for one or more NuGet packages within the solution. You should consider " +
                                 $"consolidating these package versions within Visual Studio or alternatively enable the 'Consolidate " +
                                 $"Package Versions' option in Intent in the {NugetInstaller.Identifier} module configuration." +
                                 $"{Environment.NewLine}" +
                                 $"{Environment.NewLine}" +
                                 $"{report}");
            }

            foreach (var msbuildProject in _msbuildProjects)
            {
                ValidateDependencies(msbuildProject);
            }

            foreach (var msbuildProject in _msbuildProjects)
            {
                ProcessPendingInstalls(msbuildProject);
            }
        }

        private string GetPackagesWithMultipleVersionsReport()
        {
            var allPackages = _msbuildProjects
                .SelectMany(x => x.PackageNodes)
                .Where(x => x.InstalledPackage != null)
                .ToArray();

            var packagesWithMultipleVersions = allPackages
                .Where(x => allPackages
                    .Where(y => x.InstalledPackage.Id == y.InstalledPackage.Id)
                    .Any(y => x.InstalledPackage.Version != y.InstalledPackage.Version))
                .Select(x => x.InstalledPackage.Id)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            if (!packagesWithMultipleVersions.Any())
            {
                return null;
            }

            return packagesWithMultipleVersions
                .Select(packageId => new
                {
                    PackageId = packageId,
                    VersionReport = _msbuildProjects
                        .Select(x => new
                        {
                            x.Path,
                            x.PackageNodes.SingleOrDefault(y => y.InstalledPackage?.Id == packageId)?.InstalledPackage
                                ?.Version
                        })
                        .Where(x => x.Version != null)
                        .OrderByDescending(x => x.Version)
                        .ThenBy(x => x.Path)
                        .Select(x => $"    Version {x.Version} in '{x.Path}'")
                        .Aggregate((x, y) => x + Environment.NewLine + y)
                })
                .Select(x => $"{x.PackageId} has the following versions installed:{Environment.NewLine}{x.VersionReport}")
                .Aggregate((x, y) => x + Environment.NewLine + y);
        }

        private void ConsolidateVersions(MsbuildProject project)
        {
            bool hadWork;

            do
            {
                hadWork = false;

                foreach (var node in project.PackageNodes.ToArray())
                {
                    var packageOfHighestVersion = GetPackageOfHighestVersion(node.RequiredPackage.Id);

                    if (!node.Referers.All(x => x.VersionSpec.IsSatisfiedBy(packageOfHighestVersion.Version)))
                    {
                        throw new Exception("Using solution highest package version would not satisfy all version requirements.");
                    }

                    if (node.RequiredPackage.Version != packageOfHighestVersion.Version)
                    {
                        AddOrUpdateRequiredPackage(project, node.RequiredPackage.Id, Referer.CreateFromSolutionHighestVersion(packageOfHighestVersion));
                        hadWork = true;
                    }
                }
                
            } while (hadWork);
        }

        private void ValidateDependencies(MsbuildProject msbuildProject)
        {
            foreach (var packageNode in msbuildProject.PackageNodes.ToArray())
            {
                foreach (var dependency in packageNode.RequiredPackage.GetDependencies(_nugetServices.GetTargetFrameworkName(msbuildProject.Path)))
                {
                    AddOrUpdateRequiredPackage(msbuildProject, dependency.Id, Referer.Create(packageNode, dependency.VersionSpec));
                }
            }
        }

        private void ProcessPendingInstalls(MsbuildProject project)
        {
            while (project.PackageNodes.Any(x => !x.IsInstalled()))
            {
                var pendingNodesWithDependenciesMet = project.PackageNodes
                    .Where(x => 
                        !x.IsInstalled() &&
                        x.Dependencies.All(y => y.SelfAndDependenciesInstalled()));

                foreach (var packageNode in pendingNodesWithDependenciesMet)
                {
                    _nugetServices.Install(project.Path, packageNode.RequiredPackage);
                    packageNode.UpdateInstalledPackage(packageNode.RequiredPackage);
                }
            }
        }

        private void AddOrUpdateRequiredPackage(MsbuildProject msbuildProject, string packageId, Referer referer)
        {
            var packageOfLowestVersion = GetPackageOfLowestVersion(packageId);

            var package = packageOfLowestVersion != null && referer.VersionSpec.IsSatisfiedBy(packageOfLowestVersion.Version)
                ? packageOfLowestVersion
                : _nugetServices.GetPackage(
                    packageId: packageId,
                    versionSpec: referer.VersionSpec,
                    allowPrereleaseVersions: _settings.AllowPreReleaseVersions);

            if (package == null)
            {
                _tracing.Warning($"{NugetInstaller.TracingOutputPrefix}Unable find package [{packageId} {referer.VersionSpec}] to install as requested by [{referer.Name}].");
                return;
            }

            var packageNode = msbuildProject.PackageNodes
                .SingleOrDefault(x => x.RequiredPackage.Id.Equals(packageId, StringComparison.InvariantCultureIgnoreCase));

            if (packageNode == null)
            {
                packageNode = PackageNode.Create(package);

                msbuildProject.PackageNodes.Add(packageNode);
            }

            packageNode.AddReferer(referer);

            if (packageOfLowestVersion != null)
            {
                packageNode.AddReferer(Referer.CreateFromSolutionHighestVersion(packageOfLowestVersion));
            }

            if (referer.VersionSpec.RequiresLowerThan(packageNode.RequiredPackage.Version))
            {
                throw new Exception($"Installing package [{packageNode.RequiredPackage.GetFullName()}] would require a downgrade of [{referer.Name}], as the version is too high.");
            }

            if (packageNode.RequiredPackage.Version < package.Version)
            {
                packageNode.UpdateRequiredPackage(package);
            }

            if (!packageNode.Referers.All(x => x.VersionSpec.IsSatisfiedBy(packageNode.RequiredPackage.Version)))
            {
                var preventingPackage = packageNode.Referers.First(x => !x.VersionSpec.IsSatisfiedBy(packageNode.RequiredPackage.Version));
                throw new Exception($"Unable to install [{packageNode.RequiredPackage.GetFullName()}] as the it fails to meet the version requirements of [{preventingPackage.Name}]");
            }

            foreach (var dependency in packageNode.RequiredPackage.GetDependencies(_nugetServices.GetTargetFrameworkName(msbuildProject.Path)))
            {
                AddOrUpdateRequiredPackage(msbuildProject, dependency.Id, Referer.Create(packageNode, dependency.VersionSpec));
            }
        }

        private void LoadMsbuildProjects(string solutionFilePath, IEnumerable<string> additionalProjectFilePaths)
        {
            _nugetServices.RestorePackages(solutionFilePath);

            var solutionFile = SolutionFile.Parse(solutionFilePath);
            var msbuildProjectPaths = solutionFile.ProjectsInOrder
                .Where(x => x.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat && !IsNetCoreProject(x.AbsolutePath))
                .Select(x => x.AbsolutePath)
                .Union(additionalProjectFilePaths);

            _msbuildProjects.AddRange(msbuildProjectPaths.Select(LoadMsbuildProject).Where(x => x != null));
        }

        private static bool IsNetCoreProject(string path)
        {
            var doc = XDocument.Load(path);
            return doc.XPathSelectElement("Project[@Sdk]") != null;
        }

        private MsbuildProject LoadMsbuildProject(string project)
        {
            if (string.IsNullOrWhiteSpace(project))
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (!File.Exists(project))
            {
                _tracing.Warning($"{NugetInstaller.TracingOutputPrefix}Could not find file '{project}'. This project will be ignored for now. Check your Visual Studio .sln file and/or Intent Application configuration to investigate further.");
                return null;
            }

            var msBuildProject = new MsbuildProject(project);

            foreach (var package in _nugetServices.GetInstalled(project))
            {
                msBuildProject.PackageNodes.Add(PackageNode.Create(
                    package: package,
                    installedPackage: package,
                    referer: Referer.Create(
                        name: project,
                        versionSpec: package.Version.ToMinVersionSpecInclusive())));
            }

            return msBuildProject;
        }

        private IPackage GetPackageOfHighestVersion(string packageId)
        {
            return _msbuildProjects
                .SelectMany(x => x.PackageNodes)
                .Select(x => x.RequiredPackage)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault(x => x.Id == packageId);
        }

        private IPackage GetPackageOfLowestVersion(string packageId)
        {
            return _msbuildProjects
                .SelectMany(x => x.PackageNodes)
                .Select(x => x.RequiredPackage)
                .OrderBy(x => x.Version)
                .FirstOrDefault(x => x.Id == packageId);
        }

        private MsbuildProject GetMsbuildProject(string projectFile)
        {
            return _msbuildProjects.SingleOrDefault(x => x.Path.Equals(projectFile, StringComparison.InvariantCultureIgnoreCase)) ?? LoadMsbuildProject(projectFile);
        }

        private class MsbuildProject
        {
            public string Path { get; }
            public IList<PackageNode> PackageNodes { get; }

            public MsbuildProject(string path)
            {
                Path = path;
                PackageNodes = new List<PackageNode>();
            }

            #region Equals override and related
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((MsbuildProject) obj);
            }

            private bool Equals(MsbuildProject other)
            {
                return string.Equals(Path, other.Path);
            }

            public override int GetHashCode()
            {
                return Path?.GetHashCode() ?? 0;
            }
            #endregion
        }

        public void Dispose()
        {
            ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
        }
    }
}
