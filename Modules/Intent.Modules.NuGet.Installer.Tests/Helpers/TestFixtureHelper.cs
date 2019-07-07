using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using NSubstitute;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;
using SearchOption = Intent.SoftwareFactory.Engine.SearchOption;

namespace Intent.Modules.NuGet.Installer.Tests.Helpers
{
    internal static class TestFixtureHelper
    {
        internal static IProject CreateProject(NuGetScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
        {
            return new ProjectImplementation(scheme, testVersion, testPackage, nugetPackagesToInstall);
        }

        internal static NuGetProject CreateNuGetProject(NuGetScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
        {
            return NugetInstallerFactoryExtension.DeterminePackages(
                applicationProjects: new[]
                {
                    CreateProject(scheme, testVersion, testPackage, nugetPackagesToInstall)
                },
                loadDelegate: p => File.ReadAllText(p.ProjectFile())).Projects.Single();
        }

        public static Func<IProject, string> LoadDelegate => p => File.ReadAllText(Path.GetFullPath(p.ProjectFile()));

        private class ProjectImplementation : IProject
        {
            private readonly NuGetScheme? _nuGetScheme;
            private readonly TestVersion _testVersion;
            private readonly int _number;

            public ProjectImplementation(NuGetScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
            {
                _nuGetScheme = scheme;
                _testVersion = testVersion;
                _number = (int)testPackage;

                Name = $"{(scheme.HasValue ? scheme.Value.ToString() : "null")}_{testVersion}_{_number}";
                ProjectType = new ProjectTypeImplementation(Name);
                this.InitializeVSMetaData();
                this.NugetPackages().AddRange(nugetPackagesToInstall.Select(x => new NuGetPackages(x)));
            }

            public string ProjectFile() => GetPath(_nuGetScheme, _testVersion, _number);

            public string Name { get; }

            public IProjectType ProjectType { get; }

            public IDictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

            #region throw new NotImplementedException() implementations
            public bool Equals(IProject other) => throw new NotImplementedException();
            public IEnumerable<IStereotype> Stereotypes => throw new NotImplementedException();
            public IFolder Folder => throw new NotImplementedException();
            public IEnumerable<ITemplate> FindTemplateInstances(string templateId, Func<ITemplate, bool> predicate, SearchOption searchOption) => throw new NotImplementedException();
            public ITemplate FindTemplateInstance(string templateId, Func<ITemplate, bool> predicate = null) => throw new NotImplementedException();
            public ITemplate FindTemplateInstance(string templateId, Func<ITemplate, bool> predicate, SearchOption searchOption) => throw new NotImplementedException();
            public IEnumerable<T> ResolveDecorators<T>(IHasDecorators<T> hasDecorators) where T : ITemplateDecorator => throw new NotImplementedException();
            public IEnumerable<T> ResolveDecorators<T>() where T : ITemplateDecorator => throw new NotImplementedException();
            public string ProjectLocation => throw new NotImplementedException();
            public string ProjectName => throw new NotImplementedException();
            public Guid Id => throw new NotImplementedException();
            public IApplication Application => throw new NotImplementedException();
            public IEnumerable<ITemplate> TemplateInstances => throw new NotImplementedException();
            #endregion

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

            private class ProjectTypeImplementation : IProjectType
            {
                public ProjectTypeImplementation(string name)
                {
                    Name = name;
                }

                public string Name { get; }

                #region throw new NotImplementedException() implementations
                public string Id => throw new NotImplementedException();
                public IEnumerable<IProjectTypeProperty> Properties => throw new NotImplementedException();
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
                    return null;
                case null:
                    path = $@"VerboseWithNoScheme/csproj.xml";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null);
            }

            return $"Data/{path}";
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