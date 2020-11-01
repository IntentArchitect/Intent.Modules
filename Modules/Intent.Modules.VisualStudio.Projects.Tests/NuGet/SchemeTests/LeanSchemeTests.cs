using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
    public class LeanSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new LeanSchemeProcessor();
            var project = TestFixtureHelper.CreateProject(NuGetScheme.Lean, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var doc = XDocument.Load(project.FilePath);

            // Act
            var installedPackages = sut.GetInstalledPackages(project.FilePath, doc);

            // Assert
            Assert.Collection(installedPackages, x =>
            {
                Assert.Equal("TestPackage.One", x.Key);
                Assert.Equal(x.Value.Version, VersionRange.Parse("1.0.0"));
            });
        }

        [Fact]
        public void InstallsPackage()
        {
            // Arrange
            var sut = new LeanSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(NuGetScheme.Lean, TestVersion.Low, TestPackage.One, new Dictionary<string, string>
                {
                    {"PackageToInstall.Id", "1.0.0"}
                });

            // Act
            var result = sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing);

            // Assert
            Assert.Equal(
                expected:
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""PackageToInstall.Id"" Version=""1.0.0"" />
    <PackageReference Include=""TestPackage.One"" Version=""1.0.0"" />
  </ItemGroup>

</Project>",
                actual: result);
        }

        [Fact]
        public void UpgradesPackage()
        {
            // Arrange
            var sut = new LeanSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(NuGetScheme.Lean, TestVersion.Low, TestPackage.One, new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            var result = sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing);

            // Assert
            Assert.Equal(
                expected:
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""3.0.0"" />
  </ItemGroup>

</Project>",
                actual: result);
        }

        [Theory]
        [InlineData(TestPackage.One, TestPackage.Two)]
        [InlineData(TestPackage.Two, TestPackage.One)]
        public void SortsPackageReferencesInAlphabeticalOrder(TestPackage existingPackage, TestPackage testPackageToInstall)
        {
            // Arrange
            var sut = new LeanSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(NuGetScheme.Lean, TestVersion.Low, existingPackage, new Dictionary<string, string>
            {
                { $"{nameof(TestPackage)}.{testPackageToInstall}", "1.0.0" }
            });

            // Act
            var result = sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing);

            // Assert
            Assert.Equal(
                expected:
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <ItemGroup>
    <PackageReference Include=""TestPackage.One"" Version=""1.0.0"" />
    <PackageReference Include=""TestPackage.Two"" Version=""1.0.0"" />
  </ItemGroup>

</Project>",
                actual: result);
        }
    }
}
