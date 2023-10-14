using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
public class TryCatchBlockTests
{
    [Fact]
    public async Task TryCatchWithWhenExpressionTest()
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
    
    [Fact]
    public async Task TryCatchFinallyBlocksTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddTryBlock(block => block.AddStatement("DoSomethingRisky();"));
                    method.AddCatchBlock("OutOfMemoryException", "ex", block => block.AddStatement("// What to do?"));
                    method.AddCatchBlock(block => block.AddStatement("// Catch All"));
                    method.AddFinallyBlock(block => block.AddStatement("DoFinallyStuff();"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}