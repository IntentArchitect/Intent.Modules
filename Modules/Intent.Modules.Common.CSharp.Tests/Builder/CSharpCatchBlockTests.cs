using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
public class CSharpCatchBlockTests
{
    [Fact]
    public async Task ItShouldWork()
    {
        var fileBuilder = new CSharpFile("Namespace", null)
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Method", method =>
                {
                    method.AddTryBlock();
                    method.AddCatchBlock(@catch =>
                    {
                        @catch.WithExceptionType("Exception");
                        @catch.WithParameterName("e");
                        @catch.WithWhenExpression("e.Message == \"Hello World!\"");
                    });
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}