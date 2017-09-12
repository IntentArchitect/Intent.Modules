using NuGet;
using NuGet.CommandLine;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Intent.SoftwareFactory.NuGet.NugetIntegration
{
    public class NugetServices : INugetServices
    {
        public static INugetServices Create(string solutionFile, IDictionary<string, ICanAddFileStrategy> canAddFileStrategies)
        {
            return new NugetServices(solutionFile, canAddFileStrategies);
        }

        private readonly IConsole _console;
        private readonly INuGetProjectContext _nuGetProjectContext;
        private readonly IPackageRepository _localPackageRepository;
        private readonly IPackageRepository _feedPackageRepository;
        private readonly IDictionary<string, MSBuildNuGetProject> _loadedMsBuildNuGetProjects;
        private readonly string _packagesFolder;
        private readonly IDictionary<string, ICanAddFileStrategy> _canAddFileStrategies;

        private NugetServices(string solutionFile, IDictionary<string, ICanAddFileStrategy> canAddFileStrategies)
        {
            _canAddFileStrategies = canAddFileStrategies;
            if (solutionFile == null)
            {
                throw new ArgumentNullException(nameof(solutionFile));
            }

            var directoryName = Path.GetDirectoryName(solutionFile);
            if (directoryName == null)
            {
                throw new NullReferenceException();
            }

            _packagesFolder = Path.Combine(directoryName, "packages");
            _localPackageRepository = PackageRepositoryFactory.Default.CreateRepository(_packagesFolder);
            _feedPackageRepository = PackageRepositoryFactory.Default.CreateRepository(NuGetConstants.V2FeedUrl);
            _console = new global::NuGet.Common.Console();
            _nuGetProjectContext = new ConsoleProjectContext(_console);

            _loadedMsBuildNuGetProjects = new Dictionary<string, MSBuildNuGetProject>();
        }

        public void RestorePackages(string solutionFilePath)
        {
            var fileName = Assembly.GetAssembly(typeof(Program)).Location;
            if (fileName == null)
            {
                throw new NullReferenceException();
            }

            var nugetProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = $"restore \"{solutionFilePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            nugetProcess.Start();
            nugetProcess.WaitForExit();

            if (nugetProcess.ExitCode != 0)
            {
                throw new Exception($"An issue occurred with NuGet package restore for {solutionFilePath}, full output:{Environment.NewLine}{nugetProcess.StandardOutput.ReadToEnd()}");
            }
        }

        public IPackage GetPackage(string packageId, IVersionSpec versionSpec, bool allowPrereleaseVersions)
        {
            var package =
                _localPackageRepository
                    .FindPackages(
                        packageId: packageId,
                        versionSpec: new VersionSpec(versionSpec.MinVersion), 
                        allowPrereleaseVersions: allowPrereleaseVersions,
                        allowUnlisted: false)
                    .OrderBy(x => x.Version)
                    .FirstOrDefault();

            if (package == null)
            {
                Logging.Log.Info($"Fetching NuGet package {packageId} {versionSpec}.");

                package = _feedPackageRepository
                    .FindPackages(
                        packageId: packageId,
                        versionSpec: versionSpec,
                        allowPrereleaseVersions: allowPrereleaseVersions,
                        allowUnlisted: false)
                    .OrderBy(x => x.Version)
                    .First();
            }

            if (package == null)
            {
                throw new Exception($"Could not resolve {packageId} {versionSpec}");
            }

            return package;
        }

        public bool IsInstalled(string project, IPackage package)
        {
            return GetProject(project)
                .GetInstalledPackagesAsync(new CancellationToken()).Result
                .Any(x => x.PackageIdentity.Id == package.Id && x.PackageIdentity.Version.ToString() == package.Version.ToString());
        }

        public async void Install(string project, IPackage package)
        {
            ICanAddFileStrategy canAddFileStrategy;
            _canAddFileStrategies.TryGetValue(package.Id, out canAddFileStrategy);

            Logging.Log.Info($"NuGet - Installing {package.GetFullName()} into project {project}");

            try
            {
                await GetProject(project, canAddFileStrategy).InstallPackageAsync(
                    packageIdentity: new PackageIdentity(package.Id, NuGetVersion.Parse(package.Version.ToString())),
                    downloadResourceResult: new DownloadResourceResult(package.GetStream()),
                    nuGetProjectContext: _nuGetProjectContext,
                    token: new CancellationToken());
            }
            catch (Exception e)
            {
                Logging.Log.Warning($"NuGet - Failed to install {package.GetFullName()} into project {project}: {e.Message}");
            }
            Save(project);
        }

        public async void Uninstall(string project, IPackage package)
        {
            await GetProject(project).UninstallPackageAsync(
                packageIdentity: new PackageIdentity(package.Id, NuGetVersion.Parse(package.Version.ToString())),
                nuGetProjectContext: _nuGetProjectContext,
                token: new CancellationToken());

            Save(project);
        }

        public IPackage[] GetInstalled(string project)
        {
            return GetProject(project)
                .GetInstalledPackagesAsync(new CancellationToken()).Result
                .Select(x => _localPackageRepository.FindPackage(
                    packageId: x.PackageIdentity.Id,
                    version: new global::NuGet.SemanticVersion(x.PackageIdentity.Version.ToString())))
                .ToArray();
        }

        public string GetTargetFrameworkName(string projectFile)
        {
            return GetMsbuildNuGetProjectSystem(projectFile).TargetFramework.DotNetFrameworkName;
        }

        public void CleanupPackagesFolder()
        {
            var unusedPackages = _localPackageRepository
                .GetPackages()
                .ToList()
                .Where(x => !_loadedMsBuildNuGetProjects
                    .SelectMany(y => y.Value.GetInstalledPackagesAsync(new CancellationToken()).Result)
                    .Select(y => y.PackageIdentity)
                    .Any(y => y.Id == x.Id && y.Version.ToString() == x.Version.ToString()))
                .ToArray();

            foreach (IPackage package in unusedPackages)
            {
                var pathToDelete = Path.Combine(_packagesFolder, $"{package.Id}.{package.Version}");

                // One would think Directory.Delete(path, recursive: true) would do the job, well 
                // it regularly fails, and looking online it seems the solution is try/catches with
                // retries and recursive deletes. Well guess what, Nuget has one of those
                // monstrosities already, hooray!
                LocalResourceUtils.DeleteDirectoryTree(pathToDelete, new List<string>());
            }
        }

        private void Save(string project)
        {
            GetMsbuildNuGetProjectSystem(project).Save();
        }

        private MSBuildProjectSystem GetMsbuildNuGetProjectSystem(string project)
        {
            var msbuildNuGetProjectSystem = GetProject(project).MSBuildNuGetProjectSystem as MSBuildProjectSystem;
            if (msbuildNuGetProjectSystem == null)
            {
                throw new Exception($"Could not get {nameof(MSBuildProjectSystem)} for {project}.");
            }

            return msbuildNuGetProjectSystem;
        }

        private MSBuildNuGetProject GetProject(string project, ICanAddFileStrategy canAddFileStrategy = null)
        {
            MSBuildNuGetProject msBuildNuGetProject;

            if (!_loadedMsBuildNuGetProjects.TryGetValue(project, out msBuildNuGetProject))
            {
                var msbuildNuGetProjectSystem = new CustomMsbuildProjectSystem(
                    msbuildDirectory: MsBuildUtility.GetMsbuildDirectory(null, _console),
                    projectFullPath: project,
                    projectContext: _nuGetProjectContext);

                msBuildNuGetProject = new MSBuildNuGetProject(
                    msbuildNuGetProjectSystem: msbuildNuGetProjectSystem,
                    folderNuGetProjectPath: _packagesFolder,
                    packagesConfigFolderPath: Path.GetDirectoryName(project));

                _loadedMsBuildNuGetProjects.Add(project, msBuildNuGetProject);
            }

            ((CustomMsbuildProjectSystem)msBuildNuGetProject.MSBuildNuGetProjectSystem).ApplyCanAddFileStrategy(canAddFileStrategy);

            return msBuildNuGetProject;
        }
    }
}