using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class IndentationTests
{
    [Fact]
    public async Task ItShouldNotLoseIndentation()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Method", m =>
                {
                    m.AddStatements(@"
            if (1 == 1 ||
                2 == 2)
            {
                // Nested
            }");
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}