using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Types.Api;
using NSubstitute;
using Shouldly;

namespace Intent.Modules.Common.Java.Tests.Templates
{
    public class JavaTemplateBaseExtensionMethodsTests
    {
        public class DescribeGetPackageStructure
        {
            public record Scenario(
                string Name,
                string[]? OutputTarget,
                string[]? HasFolder,
                string[]? AdditionalFolders,
                string Expected)
            {
                public object[] ToObject() => new object[] { this };
                public override string ToString() => Name;
            };

            public static readonly object[] ItShouldWorkScenarios = {
                new Scenario(
                    Name: "HasFolder is null",
                    OutputTarget: new []
                    {
                        "OutputTarget1.OutputTarget2",
                        "java"
                    },
                    HasFolder: null,
                    AdditionalFolders: new []
                    {
                        "AdditionalFolder1.AdditionalFolder2",
                        "AdditionalFolder3"
                    },
                    Expected:
                    "AdditionalFolder1.AdditionalFolder2.AdditionalFolder3").ToObject(),

                new Scenario(
                    Name: "java in HasFolder",
                    OutputTarget: new []
                    {
                        "OutputTarget1.OutputTarget2",
                        "OutputTarget3"
                    },
                    HasFolder: new []
                    {
                        "HasFolder1.HasFolder2",
                        "java",
                        "HasFolder3"
                    },
                    AdditionalFolders: new []
                    {
                        "AdditionalFolder1.AdditionalFolder2",
                        "AdditionalFolder3"
                    },
                    Expected:
                    "HasFolder3.AdditionalFolder1.AdditionalFolder2.AdditionalFolder3").ToObject(),

                new Scenario(
                    Name: "java in OutputTarget",
                    OutputTarget: new []
                    {
                        "OutputTarget1.OutputTarget2",
                        "java",
                        "OutputTarget3"
                    },
                    HasFolder: new []
                    {
                        "HasFolder1.HasFolder2",
                        "HasFolder3"
                    },
                    AdditionalFolders: new []
                    {
                        "AdditionalFolder1.AdditionalFolder2",
                        "AdditionalFolder3"
                    },
                    Expected:
                    "OutputTarget3.HasFolder1.HasFolder2.HasFolder3.AdditionalFolder1.AdditionalFolder2.AdditionalFolder3").ToObject(),

                new Scenario(
                    Name: "hyphen in path",
                    OutputTarget: new []
                    {
                        "OutputTarget1.OutputTarget2",
                        "java",
                        "OutputTarget3"
                    },
                    HasFolder: new []
                    {
                        "HasFolder1.HasFolder2",
                        "HasFolder3"
                    },
                    AdditionalFolders: new []
                    {
                        "AdditionalFolder1.AdditionalFolder2",
                        "Additional-Folder3"
                    },
                    Expected:
                    "OutputTarget3.HasFolder1.HasFolder2.HasFolder3.AdditionalFolder1.AdditionalFolder2.Additional_Folder3").ToObject(),

                new Scenario(
                    Name: "Pre part suffixed with \"java\"",
                    OutputTarget: new []
                    {
                        "testingjava",
                        "java",
                        "folder"
                    },
                    HasFolder: Array.Empty<string>(),
                    AdditionalFolders: Array.Empty<string>(),
                    Expected:
                    "folder").ToObject(),

                new Scenario(
                    Name: "Post part suffixed with \"java\"",
                    OutputTarget: new []
                    {
                        "java",
                        "folder",
                        "testingjava"
                    },
                    HasFolder: Array.Empty<string>(),
                    AdditionalFolders: Array.Empty<string>(),
                    Expected:
                    "folder.testingjava").ToObject(),

                new Scenario(
                    Name: "Pre and post part suffixed with \"java\"",
                    OutputTarget: new []
                    {
                        "testingjava",
                        "java",
                        "folder",
                        "testingjava"
                    },
                    HasFolder: Array.Empty<string>(),
                    AdditionalFolders: Array.Empty<string>(),
                    Expected:
                    "folder.testingjava").ToObject(),
            };

            [Theory]
            [MemberData(nameof(ItShouldWorkScenarios))]
            public void ItShouldWork(Scenario scenario)
            {
                // Arrange
                var outputTarget = GetOutputTarget(scenario.OutputTarget);
                var hasFolder = GetHasFolder(scenario.HasFolder);
                var additionalFolders = scenario.AdditionalFolders;

                // Act
                var result = JavaTemplateBaseExtensionMethods.GetPackageStructure(outputTarget, hasFolder, additionalFolders);

                // Assert
                result.ShouldBe(scenario.Expected);
            }
        }

        private static IOutputTarget? GetOutputTarget(params string[]? parts)
        {
            static IOutputTarget CreatePart(string part)
            {
                var outputTarget = Substitute.For<IOutputTarget>();
                outputTarget.Name.Returns(part);
                return outputTarget;
            }

            if (parts == null) return null;

            var outputTargets = parts.Select(CreatePart).ToArray();

            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.GetTargetPath().ReturnsForAnyArgs(outputTargets);
            return outputTarget;
        }

        private static IHasFolder? GetHasFolder(params string[]? parts)
        {
            static FolderModel GetPart(string part, FolderModel? parent)
            {
                var element = Substitute.For<IElement>();
                element.SpecializationType.Returns(FolderModel.SpecializationType);
                element.Name.Returns(part);

                return new FolderModel(element)
                {
                    Folder = parent
                };
            }

            if (parts == null) return null;

            var folder = parts.Aggregate(default(FolderModel), (current, part) => GetPart(part, current));

            var hasFolder = Substitute.For<IHasFolder>();
            hasFolder.Folder.Returns(folder);
            return hasFolder;
        }
    }
}