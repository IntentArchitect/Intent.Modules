using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Schemes;
using Intent.Modules.NuGet.Installer.Tests.Helpers;
using NuGet.Versioning;
using Xunit;

namespace Intent.Modules.NuGet.Installer.Tests.SchemeTests
{
    public class LeanSchemeTests
    {
        [Fact]
        public void GetsInstalledPackages()
        {
            // Arrange
            var sut = new LeanScheme();
            var project = TestFixtureHelper.CreateProject(ProjectType.LeanScheme, TestVersion.Low, TestPackage.One, new Dictionary<string, string>());
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
            var sut = new LeanScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.LeanScheme, TestVersion.Low, TestPackage.One, new Dictionary<string, string>
                {
                    {"PackageToInstall.Id", "1.0.0"}
                });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Equal(
                expected: XDocument.Parse(@"
                    <Project Sdk=""Microsoft.NET.Sdk"">
                      <ItemGroup>
                        <PackageReference Include=""PackageToInstall.Id"" Version=""1.0.0"" />
                        <PackageReference Include=""TestPackage.One"" Version=""1.0.0"" />
                      </ItemGroup>
                    </Project>").ToString(),
                actual: project.Document.ToString());
        }

        [Fact]
        public void UpgradesPackage()
        {
            // Arrange
            var sut = new LeanScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.LeanScheme, TestVersion.Low, TestPackage.One, new Dictionary<string, string>
            {
                { "TestPackage.One", "3.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Equal(
                expected: XDocument.Parse(@"
                    <Project Sdk=""Microsoft.NET.Sdk"">
                      <ItemGroup>
                        <PackageReference Include=""TestPackage.One"" Version=""3.0.0"" />
                      </ItemGroup>
                    </Project>").ToString(),
                actual: project.Document.ToString());
        }

        [Theory]
        [InlineData(TestPackage.One, TestPackage.Two)]
        [InlineData(TestPackage.Two, TestPackage.One)]
        public void SortsPackageReferencesInAlphabeticalOrder(TestPackage existingPackage, TestPackage testPackageToInstall)
        {
            // Arrange
            var sut = new LeanScheme();
            var tracing = new TestTracing();
            var project = TestFixtureHelper.CreateNuGetProject(ProjectType.LeanScheme, TestVersion.Low, existingPackage, new Dictionary<string, string>
            {
                { $"{nameof(TestPackage)}.{testPackageToInstall}", "1.0.0" }
            });

            // Act
            sut.InstallPackages(project, tracing);

            // Assert
            Assert.Equal(
                expected: XDocument.Parse(@"
                    <Project Sdk=""Microsoft.NET.Sdk"">
                      <ItemGroup>
                        <PackageReference Include=""TestPackage.One"" Version=""1.0.0"" />
                        <PackageReference Include=""TestPackage.Two"" Version=""1.0.0"" />
                      </ItemGroup>
                    </Project>").ToString(),
                actual: project.Document.ToString());
        }
    }
}
