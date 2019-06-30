using System.Collections.Generic;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Schemes;
using Intent.Modules.NuGet.Installer.Tests.Helpers;
using Xunit;

namespace Intent.Modules.NuGet.Installer.Tests.SchemeTests
{
    public class UnsupportedSchemeTests
    {
        [Fact]
        public void GetInstalledPackagesReturnsEmptyCollection()
        {
            // Arrange
            var sut = new UnsupportedScheme();
            var project = TestFixtureHelper.CreateProject(ProjectType.Unsupported, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());

            // Act
            var installedPackages = sut.GetInstalledPackages(project, null);

            // Assert
            Assert.Empty(installedPackages);
        }

        [Fact]
        public void InstallPackageCreatesWarning()
        {
            // Arrange
            var sut = new UnsupportedScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.Unsupported, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "PackageToInstall.Id", "1.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Collection(tracing.DebugEntries,
                element => Assert.Contains(
                    "Skipped",
                    element));
        }

        [Fact]
        public void UpgradePackageCreatesWarning()
        {
            // Arrange
            var sut = new UnsupportedScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.Unsupported, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Collection(tracing.DebugEntries,
                element => Assert.Contains(
                    "Skipped",
                    element));
        }
    }
}