using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Schemes;
using Intent.Modules.NuGet.Installer.Tests.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.NuGet.Installer.Tests.SchemeTests
{
    public class VerboseWithPackagesDotConfigSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new VerboseWithPackagesDotConfigScheme();
            var project = TestFixtureHelper.CreateProject(ProjectType.VerboseWithPackagesDotConfigScheme, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
            var doc = XDocument.Load(project.ProjectFile());

            // Act
            var installedPackages = sut.GetInstalledPackages(project, doc);

            // Assert
            Assert.Collection(installedPackages, x =>
            {
                Assert.Equal("TestPackage.One", x.Key);
                Assert.Equal(x.Value, SemanticVersion.Parse("1.0.0"));
            });
        }

        [Fact]
        public void InstallPackageCreatesWarning()
        {
            // Arrange
            var sut = new VerboseWithPackagesDotConfigScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.VerboseWithPackagesDotConfigScheme, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "PackageToInstall.Id", "1.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

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
            var sut = new VerboseWithPackagesDotConfigScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.VerboseWithPackagesDotConfigScheme, TestVersion.Low, TestPackage.One, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Collection(tracing.WarningEntries,
                element => Assert.Contains(
                    "https://blog.nuget.org/20180409/migrate-packages-config-to-package-reference.html",
                    element));
        }
    }
}