using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.Modules.NuGet.Installer.Tests.Helpers;
using Xunit;
using static Intent.Modules.NuGet.Installer.Tests.Helpers.TestFixtureHelper;

namespace Intent.Modules.NuGet.Installer.Tests
{
    public class NugetInstallerFactoryExtensionTests
    {
        [Fact]
        public void ConsolidatesInstalledPackageVersions()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = new NugetInstallerFactoryExtension().Configure(true, false);
            var saved = new List<NuGetProject>();
            var projects = new[]
            {
                CreateProject(ProjectType.LeanScheme, TestVersion.High, 1, new Dictionary<string, string>()),
                CreateProject(ProjectType.LeanScheme, TestVersion.Low, 1, new Dictionary<string, string>()),
            };

            // Act
            sut.Execute(projects, tracing, project => saved.Add(project), LoadDelegate);

            // Assert
            Assert.Collection(saved, elementInspectors: new Action<NuGetProject>[] {
                x => Assert.Equal(
                    XDocument.Parse(@"
                        <Project Sdk=""Microsoft.NET.Sdk"">
                          <ItemGroup>
                            <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
                          </ItemGroup>
                        </Project>").ToString(),
                    x.Document.ToString()),
                x => Assert.Equal(
                    XDocument.Parse(@"
                        <Project Sdk=""Microsoft.NET.Sdk"">
                          <ItemGroup>
                            <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
                          </ItemGroup>
                        </Project>").ToString(),
                    x.Document.ToString())
            });
        }
        [Fact]
        public void ConsolidatesInstallVersionsToHighestExisting()
        {
            // Arrange
            var tracing = new TestTracing();
            var sut = new NugetInstallerFactoryExtension().Configure(true, false);
            var saved = new List<NuGetProject>();
            var projects = new[]
            {
                CreateProject(ProjectType.LeanScheme, TestVersion.High, 1, new Dictionary<string, string>()),
                CreateProject(ProjectType.LeanScheme, TestVersion.High, 2, new Dictionary<string, string>
                {
                    { "TestPackage.One", "1.0.0" }
                }),
            };

            // Act
            sut.Execute(projects, tracing, project => saved.Add(project), LoadDelegate);

            // Assert
            Assert.Collection(saved, elementInspectors: new Action<NuGetProject>[] {
                x => Assert.Equal(
                    XDocument.Parse(@"
                        <Project Sdk=""Microsoft.NET.Sdk"">
                          <ItemGroup>
                            <PackageReference Include=""TestPackage.One"" Version=""2.0.0"" />
                            <PackageReference Include=""TestPackage.Two"" Version=""2.0.0"" />
                          </ItemGroup>
                        </Project>").ToString(),
                    x.Document.ToString())
            });
        }
    }
}
