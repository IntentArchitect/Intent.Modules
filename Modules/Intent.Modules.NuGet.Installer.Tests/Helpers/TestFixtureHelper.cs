using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Intent.MetaModel.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;
using SearchOption = Intent.SoftwareFactory.Engine.SearchOption;

namespace Intent.Modules.NuGet.Installer.Tests.Helpers
{
    internal static class TestFixtureHelper
    {
        internal static IProject CreateProject(ProjectType? projectType, TestVersion testVersion, int number, IDictionary<string, string> nugetPackagesToInstall)
        {
            return new ProjectImplementation(projectType, testVersion, number, nugetPackagesToInstall);
        }

        internal static NuGetProject CreateNuGetProject(ProjectType? projectType, TestVersion testVersion, int number, IDictionary<string, string> nugetPackagesToInstall)
        {
            return NugetInstallerFactoryExtension.DeterminePackages(
                applicationProjects: new[]
                {
                    CreateProject(projectType, testVersion, number, nugetPackagesToInstall)
                },
                loadDelegate: p => XDocument.Load(p.ProjectFile())).Projects.Single();
        }

        public static Func<IProject, XDocument> LoadDelegate => p => XDocument.Load(Path.GetFullPath(p.ProjectFile()));

        private class ProjectImplementation : IProject
        {
            private readonly ProjectType? _projectType;
            private readonly TestVersion _testVersion;
            private readonly int _number;

            public ProjectImplementation(ProjectType? projectType, TestVersion testVersion, int number, IDictionary<string, string> nugetPackagesToInstall)
            {
                _projectType = projectType;
                _testVersion = testVersion;
                _number = number;

                Name = $"{(projectType.HasValue ? projectType.Value.ToString() : "null")}_{testVersion}_{number}";
                ProjectType = new ProjectTypeImplementation(Name);
                this.InitializeVSMetaData();
                this.NugetPackages().AddRange(nugetPackagesToInstall.Select(x => new NuGetPackages(x)));
            }

            public string ProjectFile() => GetPath(_projectType, _testVersion, _number);

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

                #region throw new NotImplementedException() implementations
                public string TargetFramework => throw new NotImplementedException();
                public bool CanAddFile(string file) => throw new NotImplementedException();
                public IList<AssemblyRedirectInfo> AssemblyRedirects => throw new NotImplementedException();
                public string[] PrivateAssets => throw new NotImplementedException();
                public string[] IncludeAssets => throw new NotImplementedException();
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

        internal static string GetPath(ProjectType? projectType, TestVersion testVersion, int number)
        {
            string path;
            switch (projectType)
            {
                case ProjectType.LeanScheme:
                    path = $@"{projectType}/{testVersion}Version{number}.xml";
                    break;
                case ProjectType.VerboseWithPackageReferenceScheme:
                    path = $@"{projectType}/{testVersion}Version{number}.xml";
                    break;
                case ProjectType.VerboseWithPackagesDotConfigScheme:
                    path = $@"{projectType}/{testVersion}Version{number}/csproj.xml";
                    break;
                case ProjectType.Unsupported:
                    return null;
                case null:
                    path = $@"VerboseWithNoScheme/csproj.xml";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectType), projectType, null);
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
    }
}