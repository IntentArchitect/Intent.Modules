using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.SchemeTests
{
    public class VerboseWithPackagesDotConfigSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new VerboseWithPackagesDotConfigSchemeProcessor();
            var project = TestFixtureHelper.CreateProject(NuGetScheme.VerboseWithPackagesDotConfig, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
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
        public void InstallPackageCreatesWarning()
        {
            // Arrange
            var sut = new VerboseWithPackagesDotConfigSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(NuGetScheme.VerboseWithPackagesDotConfig, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "PackageToInstall.Id", "1.0.0" }
            });

            // Act
            var result = sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing);

            // Assert
            Assert.Collection(tracing.WarningEntries,
                element => Assert.Contains(
                    "https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html",
                    element));
        }

        [Fact]
        public void UpgradePackageCreatesWarning()
        {
            // Arrange
            var sut = new VerboseWithPackagesDotConfigSchemeProcessor();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(NuGetScheme.VerboseWithPackagesDotConfig, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            sut.InstallPackages(
                project.Content,
                project.RequestedPackages,
                project.InstalledPackages,
                project.Name,
                tracing);

            // Assert
            Assert.Collection(tracing.WarningEntries,
                element => Assert.Contains(
                    "https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html",
                    element));
        }
    }
}