using Intent.SoftwareFactory.NuGet.NugetIntegration;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.SoftwareFactory.Templates;
using Microsoft.Build.Evaluation;
using IPackage = NuGet.IPackage;

namespace Intent.SoftwareFactory.NuGet.Managers
{
    public class NugetManager : IDisposable
    {
        private readonly string _solutionFilePath;
        private readonly bool _allowPreReleaseVersions;
        private readonly INugetServices _nugetServices;
        private readonly IList<MsbuildProject> _msbuildProjects = new List<MsbuildProject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solutionFilePath"></param>
        /// <param name="allowPreReleaseVersions"></param>
        /// <param name="projectFilePaths">Project files which may not be saved to the .sln file yet, but should be processed as well.</param>
        public NugetManager(string solutionFilePath, bool allowPreReleaseVersions = false, IEnumerable<string> projectFilePaths = null, IDictionary<string, ICanAddFileStrategy> canAddFileStrategies = null)
        {
            if (solutionFilePath == null)
            {
                throw new ArgumentNullException(nameof(solutionFilePath));
            }

            if (projectFilePaths == null)
            {
                projectFilePaths = new string[0];
            }

            if (!File.Exists(solutionFilePath))
            {
                throw new ArgumentException($"File '{solutionFilePath}' not found.", nameof(solutionFilePath));
            }

            _solutionFilePath = solutionFilePath;
            _allowPreReleaseVersions = allowPreReleaseVersions;
            _nugetServices = NugetServices.Create(solutionFilePath, canAddFileStrategies);

            LoadMsbuildProjects(solutionFilePath, projectFilePaths);
        }

        public void AddNugetPackage(string projectFile, string packageId, string minVersionInclusive)
        {
            AddOrUpdateRequiredPackage(
                msbuildProject: GetMsbuildProject(projectFile),
                packageId: packageId,
                referer: Referer.Create("Software Factory", Utility.GetMinVersionSpecInclusive(minVersionInclusive)));
        }

        public void RestorePackages()
        {
            _nugetServices.RestorePackages(_solutionFilePath);
        }

        public void CleanupPackagesFolder()
        {
            _nugetServices.CleanupPackagesFolder();
        }

        public void ProcessPendingInstalls()
        {
            foreach (var msbuildProject in _msbuildProjects)
            {
                ConsolidateVersions(msbuildProject);
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
            var packageOfHighestVersion = GetPackageOfHighestVersion(packageId);

            var package = packageOfHighestVersion != null && referer.VersionSpec.IsSatisfiedBy(packageOfHighestVersion.Version)
                ? packageOfHighestVersion
                : _nugetServices.GetPackage(
                    packageId: packageId,
                    versionSpec: referer.VersionSpec,
                    allowPrereleaseVersions: _allowPreReleaseVersions);

            var packageNode = msbuildProject.PackageNodes
                .SingleOrDefault(x => x.RequiredPackage.Id.Equals(packageId, StringComparison.InvariantCultureIgnoreCase));

            if (packageNode == null)
            {
                packageNode = PackageNode.Create(package);

                msbuildProject.PackageNodes.Add(packageNode);
            }

            packageNode.AddReferer(referer);

            if (packageOfHighestVersion != null)
            {
                packageNode.AddReferer(Referer.CreateFromSolutionHighestVersion(packageOfHighestVersion));
            }

            if (referer.VersionSpec.RequiresLowerThan(packageNode.RequiredPackage.Version))
            {
                throw new Exception("Installing package would require a downgrade.");
            }

            if (packageNode.RequiredPackage.Version < package.Version)
            {
                packageNode.UpdateRequiredPackage(package);
            }

            if (!packageNode.Referers.All(x => x.VersionSpec.IsSatisfiedBy(packageNode.RequiredPackage.Version)))
            {
                throw new Exception("Could not resolve dependencies.");
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
                .Where(x => x.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                .Select(x => x.AbsolutePath)
                .Union(additionalProjectFilePaths);

            foreach (var project in msbuildProjectPaths)
            {
                _msbuildProjects.Add(LoadMsbuildProject(project));
            }
        }

        private MsbuildProject LoadMsbuildProject(string project)
        {
            if (string.IsNullOrWhiteSpace(project))
            {
                throw new ArgumentNullException(nameof(project));
            }

#warning this should possbily be a warning and continue, this scenario can happen when you remove a project and have not manually updated the sln file
            if (!File.Exists(project))
            {
                throw new FileNotFoundException(project);
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
