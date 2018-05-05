using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.SoftwareFactory.Engine;
using NuGet;
using NuGet.CommandLine;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.NugetIntegration
{
    public class NugetServices : INugetServices
    {
        public static INugetServices Create(string solutionFile, IDictionary<string, ICanAddFileStrategy> canAddFileStrategies, ITracing tracing)
        {
            return new NugetServices(solutionFile, canAddFileStrategies, tracing);
        }

        private readonly IConsole _console;
        private readonly INuGetProjectContext _nuGetProjectContext;
        private readonly IPackageRepository _localPackageRepository;
        private readonly IPackageRepository _feedPackageRepository;
        private readonly IDictionary<string, MSBuildNuGetProject> _loadedMsBuildNuGetProjects;
        private readonly string _packagesFolder;
        private readonly IDictionary<string, ICanAddFileStrategy> _canAddFileStrategies;
        private readonly ITracing _tracing;

        private NugetServices(string solutionFile, IDictionary<string, ICanAddFileStrategy> canAddFileStrategies, ITracing tracing)
        {
            _canAddFileStrategies = canAddFileStrategies;
            _tracing = tracing;
            if (solutionFile == null)
            {
                throw new ArgumentNullException(nameof(solutionFile));
            }

            _packagesFolder = GetPackagesRepositoryPath(solutionFile);
            _localPackageRepository = PackageRepositoryFactory.Default.CreateRepository(_packagesFolder);
            _feedPackageRepository = PackageRepositoryFactory.Default.CreateRepository(NuGetConstants.V2FeedUrl);
            _console = new global::NuGet.Common.Console();
            _nuGetProjectContext = new ConsoleProjectContext(_console);

            _loadedMsBuildNuGetProjects = new Dictionary<string, MSBuildNuGetProject>();
        }

        private string GetPackagesRepositoryPath(string solutionFile)
        {
            // See https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior for full details, but the
            // short version is that the local packages folder's location can be configured through configuration files.

            var directoryName = Path.GetDirectoryName(solutionFile);
            if (directoryName == null)
            {
                throw new NullReferenceException();
            }

            var directoryInfo = new DirectoryInfo(directoryName);
            string customRepositoryPath;
            do
            {
                if (TryGetCustomRepositoryPath(directoryInfo.FullName, out customRepositoryPath))
                    break;

                directoryInfo = directoryInfo.Parent;
            } while (directoryInfo != null);

            return customRepositoryPath ?? Path.Combine(directoryName, "packages");
        }

        private bool TryGetCustomRepositoryPath(string x, out string customRepositoryPath)
        {
            customRepositoryPath = null;

            var configFile = Path.Combine(x, "nuget.config");
            if (!File.Exists(configFile))
            {
                return false;
            }

            XDocument doc;
            try
            {
                doc = XDocument.Parse(File.ReadAllText(configFile));
            }
            catch (Exception)
            {
                return false;
            }

            if (doc.Root == null)
            {
                return false;
            }

            var repositoryPath = doc
                .XPathSelectElement("/configuration/config/add[@key='repositoryPath']")
                ?.Attribute("value")
                ?.Value;

            if (string.IsNullOrWhiteSpace(repositoryPath))
            {
                return false;
            }

            _tracing.Debug($"{NugetInstaller.TracingOutputPrefix}Found repositoryPath with value '{repositoryPath}' in '{configFile}'");

            customRepositoryPath = Path.IsPathRooted(repositoryPath)
                ? repositoryPath
                : Path.Combine(x, repositoryPath);

            return true;
        }

        public void RestorePackages(string solutionFilePath)
        {
            _tracing.Info($"{NugetInstaller.TracingOutputPrefix}Running Package restore for solution '{solutionFilePath}'");
            var result =
                RunNuGetManager(
                    solutionFilePath: solutionFilePath,
                    onOutputDataReceived: x => _tracing.Info($"{NugetInstaller.TracingOutputPrefix}{x}"),
                    onErrorDataReceived: x => _tracing.Failure($"{NugetInstaller.TracingOutputPrefix}{x}"))
                .Result;

            if (result == 0)
            {
                _tracing.Info($"{NugetInstaller.TracingOutputPrefix}Package restore for solution '{solutionFilePath}' complete.");
            }
            else
            {
                throw new Exception($"{NugetInstaller.TracingOutputPrefix}Package restore process for solution '{solutionFilePath}' exited with return code {result}. See above log for more information.");
            }
        }

        private static Task<int> RunNuGetManager(string solutionFilePath, Action<string> onOutputDataReceived, Action<string> onErrorDataReceived)
        {
            var tcs = new TaskCompletionSource<int>();

            var fileName = Assembly.GetAssembly(typeof(Program)).Location;
            if (fileName == null)
            {
                throw new NullReferenceException();
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = $"restore \"{solutionFilePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(args.Data) || onOutputDataReceived == null) return;
                onOutputDataReceived(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(args.Data) || onErrorDataReceived == null) return;
                onErrorDataReceived(args.Data);
            };
            
            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
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
                _tracing.Info($"{NugetInstaller.TracingOutputPrefix}Fetching package {packageId} {versionSpec}.");

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

            _tracing.Info($"{NugetInstaller.TracingOutputPrefix}Installing {package.GetFullName()} into project {project}");

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
                _tracing.Warning($"{NugetInstaller.TracingOutputPrefix}Failed to install {package.GetFullName()} into project {project}: {e.Message}");
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

        private CustomMsbuildProjectSystem GetMsbuildNuGetProjectSystem(string project)
        {
            var msbuildNuGetProjectSystem = GetProject(project).MSBuildNuGetProjectSystem as CustomMsbuildProjectSystem;
            if (msbuildNuGetProjectSystem == null)
            {
                throw new Exception($"Could not get {nameof(CustomMsbuildProjectSystem)} for {project}.");
            }

            return msbuildNuGetProjectSystem;
        }

        private MSBuildNuGetProject GetProject(string project, ICanAddFileStrategy canAddFileStrategy = null)
        {
            MSBuildNuGetProject msBuildNuGetProject;

            if (!_loadedMsBuildNuGetProjects.TryGetValue(project, out msBuildNuGetProject))
            {
                var msbuildNuGetProjectSystem = new CustomMsbuildProjectSystem(
                    projectFullPath: project,
                    projectContext: _nuGetProjectContext,
                    ignoreMissingImports: true);

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