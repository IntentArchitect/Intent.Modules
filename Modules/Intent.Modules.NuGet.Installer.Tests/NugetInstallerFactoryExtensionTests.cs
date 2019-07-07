using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Tests.Helpers;
using Xunit;
using static Intent.Modules.NuGet.Installer.Tests.Helpers.TestFixtureHelper;

namespace Intent.Modules.NuGet.Installer.Tests
{
    public class NugetInstallerFactoryExtensionTests
    {
        [Theory]
        [InlineData(NuGetScheme.Lean)]
        [InlineData(NuGetScheme.VerboseWithPackageReference)]
        [InlineData(NuGetScheme.VerboseWithPackagesDotConfig)]
        public void ResolvesCorrectly(object untypedNuGetScheme)
        {
            var nuGetScheme = (NuGetScheme) untypedNuGetScheme;

            // Arrange
            var project = CreateProject(nuGetScheme, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var document = XDocument.Load(project.ProjectFile());

            // Act
            var result = NugetInstallerFactoryExtension.ResolveNuGetScheme(document.Root);

            // Assert
            Assert.Equal(nuGetScheme, result);
        }

        [Fact]
        public void ConsolidatationNotTriggeredWhenVersionsAreTheSame()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = GetNuGetInstaller(true, false);
            var projects = new[]
            {
                CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>()),
                CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>()),
            };

            // Act
            sut.Execute(projects, tracing, (filePath, content) => { }, LoadDelegate);

            // Assert
            Assert.Empty(tracing.InfoEntries);
        }

        [Fact]
        public void ConsolidatesInstalledPackageVersions()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = GetNuGetInstaller(true, false);
            var saved = new List<(string path, string content)>();
            var project1 = CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var project2 = CreateProject(NuGetScheme.Lean, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var projects = new[] { project1, project2 };

            // Act
            sut.Execute(projects, tracing, (path, content) => saved.Add((path, content)), LoadDelegate);

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.ProjectFile(), nuGetProject.path);
                Assert.Equal(
                    XDocument.Parse(
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
  </ItemGroup>

</Project>", LoadOptions.PreserveWhitespace).ToString(),
                    nuGetProject.content);
            });
        }

        [Fact]
        public void ConsolidateInstallsVersionsToHighestExisting()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = GetNuGetInstaller(true, false);
            var saved = new List<(string path, string content)>();
            var project1 = CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var project2 = CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.Two, new Dictionary<string, string>
            {
                { "TestPackage.One", "1.0.0" }
            });
            var projects = new[] { project1, project2 };

            // Act
            sut.Execute(projects, tracing, (path, content) => saved.Add((path, content)), LoadDelegate);

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.ProjectFile(), nuGetProject.path);
                Assert.Equal(
                    XDocument.Parse(
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
    <PackageReference Include=""TestPackage.Two"" Version=""2.0.0"" />
  </ItemGroup>

</Project>", LoadOptions.PreserveWhitespace).ToString(),
                    nuGetProject.content);
            });
        }
    }
}
