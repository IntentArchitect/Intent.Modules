using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Schemes;
using Intent.Modules.NuGet.Installer.Tests.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.NuGet.Installer.Tests.SchemeTests
{
    public class VerboseWithPackageReferenceSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new VerboseWithPackageReferencesScheme();
            var project = TestFixtureHelper.CreateProject(ProjectType.VerboseWithPackageReferenceScheme, TestVersion.Low, 1, new Dictionary<string, string>());
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
        public void InstallsPackage()
        {
            // Arrange
            var sut = new VerboseWithPackageReferencesScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.VerboseWithPackageReferenceScheme, TestVersion.Low, 1, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "PackageToInstall.Id", "1.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Equal(
                expected: XDocument.Parse(@"
                    <Project>
                      <ItemGroup>
                        <PackageReference Include=""TestPackage.One"">
                          <Version>1.0.0</Version>
                        </PackageReference>
                        <PackageReference Include=""PackageToInstall.Id"">
                          <Version>1.0.0</Version>
                        </PackageReference>
                      </ItemGroup>
                    </Project>").ToString(),
                actual: project.Document.ToString());
        }

        [Fact]
        public void UpgradesPackage()
        {
            // Arrange
            var sut = new VerboseWithPackageReferencesScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.VerboseWithPackageReferenceScheme, TestVersion.Low, 1, nugetPackagesToInstall: new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Equal(
                expected: XDocument.Parse(@"
                    <Project>
                      <ItemGroup>
                        <PackageReference Include=""TestPackage.One"">
                          <Version>3.0.0</Version>
                        </PackageReference>
                      </ItemGroup>
                    </Project>").ToString(),
                actual: project.Document.ToString());
        }
    }
}