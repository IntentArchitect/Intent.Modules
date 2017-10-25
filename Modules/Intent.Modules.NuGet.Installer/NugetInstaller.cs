using Intent.Modules.NuGet.Installer.Managers;
using Intent.Modules.Common.Plugins;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intent.Modules.NuGet.Installer
{
    public class NugetInstaller : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id
        {
            get
            {
                return "Intent.NugetInstaller";
            }
        }

        public NugetInstaller()
        {
            Order = 100;
        }


        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                Run(application);
            }
        }

        public void Run(IApplication application)
        {
            Logging.Log.Info($"NuGet - Start processesing Packages");

            var addFileBehaviours = application.Projects
                .SelectMany(x =>  x.NugetPackages())
                .GroupBy(x => x.Name)
                .Distinct()
                .ToDictionary(x => x.Key, x => new CanAddFileStrategy(x) as ICanAddFileStrategy);

            using (var nugetManager = new NugetManager(
                solutionFilePath: application.GetSolutionPath(),
                allowPreReleaseVersions: true,
                projectFilePaths: application.Projects.Select(x => Path.GetFullPath(Path.Combine(x.ProjectFile()))),
                canAddFileStrategies: addFileBehaviours))
            {
                foreach (var project in application.Projects)
                {
                    var projectFile = Path.GetFullPath(project.ProjectFile());

                    Logging.Log.Info($"NuGet - Determining Packages for installation - { project.ProjectFile() }");

                    var nugetPackages = project.NugetPackages();
                    foreach (var nugetPackageInfo in nugetPackages)
                    {
                        nugetManager.AddNugetPackage(
                            projectFile: projectFile,
                            packageId: nugetPackageInfo.Name,
                            minVersionInclusive: nugetPackageInfo.Version);
                    }
                }

                Logging.Log.Info($"NuGet - Processing pending installs...");
                nugetManager.ProcessPendingInstalls();

                Logging.Log.Info($"NuGet - Cleaning up packages folders...");
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