using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Templates;
using NSubstitute;
using IApplication = Intent.Engine.IApplication;
using SearchOption = Intent.Engine.SearchOption;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers
{
    internal static class TestFixtureHelper
    {
        internal static ProjectImplementation CreateProject(NuGetScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
        {
            return new ProjectImplementation(scheme, testVersion, testPackage, nugetPackagesToInstall);
        }

        internal static NuGetProject CreateNuGetProject(NuGetScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
        {
            return NugetInstallerFactoryExtension.DetermineProjectNugetPackageInfo(CreateProject(scheme, testVersion, testPackage, nugetPackagesToInstall));
        }

        internal class ProjectImplementation : IVisualStudioProjectTemplate
        {
            private readonly IDictionary<string, string> _nugetPackagesToInstall;

            public ProjectImplementation(NuGetScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
            {
                _nugetPackagesToInstall = nugetPackagesToInstall;

                Name = $"{(scheme.HasValue ? scheme.Value.ToString() : "null")}_{testVersion}_{(int)testPackage}";
                FilePath = GetPath(scheme, testVersion, (int)testPackage);
            }

            public string ProjectId => Name;

            public string Name { get; }

            public string FilePath { get; }

            public string LoadContent()
            {
                return File.ReadAllText(FilePath);
            }

            public IEnumerable<INugetPackageInfo> RequestedNugetPackages()
            {
                return _nugetPackagesToInstall.Select(x => new NuGetPackages(x));
            }

            private class NuGetPackages : INugetPackageInfo
            {
                public NuGetPackages(KeyValuePair<string, string> package)
                {
                    Name = package.Key;
                    Version = package.Value;
                }

                public string Name { get; }
                public string Version { get; }
                public string[] PrivateAssets => new string[0];
                public string[] IncludeAssets => new string[0];

                #region throw new NotImplementedException() implementations
                public string TargetFramework => throw new NotImplementedException();
                public bool CanAddFile(string file) => throw new NotImplementedException();
                public IList<AssemblyRedirectInfo> AssemblyRedirects => throw new NotImplementedException();
                #endregion
            }
        }

        internal static string GetPath(NuGetScheme? scheme, TestVersion testVersion, int number)
        {
            string path;
            switch (scheme)
            {
                case NuGetScheme.Lean:
                    path = $@"{scheme}/{testVersion}Version{number}.xml";
                    break;
                case NuGetScheme.VerboseWithPackageReference:
                    path = $@"{scheme}/{testVersion}Version{number}.xml";
                    break;
                case NuGetScheme.VerboseWithPackagesDotConfig:
                    path = $@"{scheme}/{testVersion}Version{number}/csproj.xml";
                    break;
                case NuGetScheme.Unsupported:
                case null:
                    path = $@"VerboseWithNoScheme/csproj.xml";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null);
            }

            return $"NuGet/Data/{path}";
        }

        public static NugetInstallerFactoryExtension Configure(
            this NugetInstallerFactoryExtension nugetInstaller,
            bool consolidatePackageVersions,
            bool warnOnMultipleVersionsOfSamePackage)
        {
            nugetInstaller.Configure(new Dictionary<string, string>
                {
                    { "Consolidate Package Versions", consolidatePackageVersions.ToString() },
                    { "Warn On Multiple Versions of Same Package", warnOnMultipleVersionsOfSamePackage.ToString() }
                });

            return nugetInstaller;
        }

        public static NugetInstallerFactoryExtension GetNuGetInstaller(
            bool consolidatePackageVersions,
            bool warnOnMultipleVersionsOfSamePackage)
        {
            var nugetInstallerFactoryExtension = new NugetInstallerFactoryExtension(Substitute.For<ISoftwareFactoryEventDispatcher>(), GetChangeManager());
            nugetInstallerFactoryExtension.Configure(consolidatePackageVersions, warnOnMultipleVersionsOfSamePackage);

            return nugetInstallerFactoryExtension;
        }

        public static IChanges GetChangeManager()
        {
            var changeManager = Substitute.For<IChanges>();

            changeManager.FindChange(Arg.Any<string>()).Returns(x => null);

            return changeManager;
        }
    }
}