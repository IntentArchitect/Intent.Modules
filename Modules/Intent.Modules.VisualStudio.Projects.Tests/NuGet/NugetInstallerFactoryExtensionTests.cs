using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet
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
            var project = TestFixtureHelper.CreateProject(nuGetScheme, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var document = XDocument.Load(project.FilePath);

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
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);
            var projects = new[]
            {
                TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>()),
                TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>()),
            };

            // Act
            sut.Execute(projects, tracing, (filePath, content) => { });

            // Assert
            Assert.Empty(tracing.InfoEntries);
        }

        [Fact]
        public void ConsolidatesInstalledPackageVersions()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);
            var saved = new List<(string path, string content)>();
            var project1 = TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var project2 = TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var projects = new[] { project1, project2 };

            // Act
            sut.Execute(projects, tracing, (path, content) => saved.Add((path, content)));

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.FilePath, nuGetProject.path);
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
            var sut = TestFixtureHelper.GetNuGetInstaller(true, false);
            var saved = new List<(string path, string content)>();
            var project1 = TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.One, new Dictionary<string, string>());
            var project2 = TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.High, TestPackage.Two, new Dictionary<string, string>
            {
                { "TestPackage.One", "1.0.0" }
            });
            var projects = new[] { project1, project2 };

            // Act
            sut.Execute(projects, tracing, (path, content) => saved.Add((path, content)));

            // Assert
            Assert.Collection(saved, nuGetProject =>
            {
                Assert.Equal(project2.FilePath, nuGetProject.path);
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
