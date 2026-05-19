using Intent.Modules.Common.TypeScript.Builder;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests.Builder
{
    public class TypescriptFileTests
    {
        [Fact]
        public void ItShouldWork()
        {
            // Arrange
            var builder = new TypescriptFile(string.Empty, null);
            builder
                .AddImport("myType", "my-source")
                .AddImport("myOtherType", "my-source")
                .AddImport("*", "cdk", "aws-cdk-lib")
                .AddClass("MyAppStack", c => c
                    .Export()
                    .ExtendsClass("Stack")
                    .AddConstructor(con => con
                        .AddParameter("scope", "Construct", cp => cp
                            .Optional()
                            .WithPrivateReadonlyFieldAssignment()
                        )
                        .AddParameter("id", "string")
                        .AddParameter("props", "StackProps", c1 => c1.Optional())
                        .CallsSuper(s => s
                            .AddArgument("scope")
                            .AddArgument("id")
                            .AddArgument("props")
                        )
                        .AddStatement(@"const hello = new lambda.Function(this, 'HelloHandler', {
              runtime: lambda.Runtime.NODEJS_14_X,
              code: lambda.Code.fromAsset('lambda'),
              handler: 'hello.handler'
            });")
                    )
                    .AddField("name", "string", f => f
                        .PrivateReadOnly()
                    )
                    .AddMethod("myMethod", "void", m => m
                        .AddStatement("var j = '';")
                    )
                )
                ;

            // Act
            builder.StartBuild();
            builder.CompleteBuild();
            var result = builder.ToString();
        }

        [Fact]
        public void ImportsAreSortedByBestPractices()
        {
            // Arrange
            var builder = new TypescriptFile(string.Empty, null);
            builder
                .AddImport("utils", "./utils")           // Relative import
                .AddImport("Component", "react")         // External import
                .AddImport("Config", "@config/settings") // Absolute import
                .AddImport("helpers", "../helpers")      // Relative import
                .AddImport("axios", "axios")             // External import
                .StartBuild()
                .CompleteBuild();

            // Act
            var result = builder.ToString();

            // Assert
            // External imports should come first (axios, react), then absolute imports (@config/settings), then relative imports (../helpers, ./utils)
            var axiosIndex = result.IndexOf("import");
            var reactIndex = result.IndexOf("import", axiosIndex + 1);
            var absoluteIndex = result.IndexOf("import", reactIndex + 1);
            var relativeUpIndex = result.IndexOf("import", absoluteIndex + 1);
            var relativeDotIndex = result.IndexOf("import", relativeUpIndex + 1);

            Assert.True(axiosIndex < reactIndex, "axios should come before react");
            Assert.True(reactIndex < absoluteIndex, "react should come before @config imports");
            Assert.True(absoluteIndex < relativeUpIndex, "@config should come before relative imports");
            Assert.True(relativeUpIndex < relativeDotIndex, "../helpers should come before ./utils");
        }
    }
}