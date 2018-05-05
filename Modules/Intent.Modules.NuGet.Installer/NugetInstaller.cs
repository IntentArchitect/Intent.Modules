using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public override string Id => "Intent.NugetInstaller";

        public NugetInstaller()
        {
            Order = 100;
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
            tracing.Info($"NuGet - Start processesing Packages");

            foreach (var project in application.Projects.Where(x => x.ProjectFile() == null))
            {
                tracing.Debug($"NuGet - Skipped processing project '{project.Name}' as its type '{project.ProjectType.Name}' is unsupported.");
            }

            var applicableProjects = application.Projects
                .Where(x => x.ProjectFile() != null)
                .ToArray();

            var addFileBehaviours = applicableProjects
                .SelectMany(x =>  x.NugetPackages())
                .GroupBy(x => x.Name)
                .Distinct()
                .ToDictionary(x => x.Key, x => new CanAddFileStrategy(x) as ICanAddFileStrategy);

            using (var nugetManager = new NugetManager(
                solutionFilePath: application.GetSolutionPath(),
                tracing: tracing,
                allowPreReleaseVersions: true,
                projectFilePaths: applicableProjects.Select(x => Path.GetFullPath(x.ProjectFile())),
                canAddFileStrategies: addFileBehaviours))
            {
                foreach (var project in applicableProjects)
                {
                    var projectFile = Path.GetFullPath(project.ProjectFile());

                    tracing.Info($"NuGet - Determining Packages for installation - { project.ProjectFile() }");

                    var nugetPackages = project.NugetPackages();
                    foreach (var nugetPackageInfo in nugetPackages)
                    {
                        nugetManager.AddNugetPackage(
                            projectFile: projectFile,
                            packageId: nugetPackageInfo.Name,
                            minVersionInclusive: nugetPackageInfo.Version);
                    }
                }

                tracing.Info($"NuGet - Processing pending installs...");
                nugetManager.ProcessPendingInstalls();

                tracing.Info($"NuGet - Cleaning up packages folders...");
                nugetManager.CleanupPackagesFolder();
            }
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