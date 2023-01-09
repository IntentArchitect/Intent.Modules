using Intent.Modules.Common.TypeScript.Builder;

namespace Intent.Modules.Common.TypeScript.Tests.Builder
{
    public class TypescriptFileTests
    {
        [Fact]
        public void ItShouldWork()
        {
            // Arrange
            var builder = new TypescriptFile(string.Empty);
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
    }
}