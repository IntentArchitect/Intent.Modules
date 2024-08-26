using System.Threading.Tasks;
using Intent.Modules.Common.FileBuilders.IndentedFileBuilder;
using Intent.Modules.Common.Tests.FileBuilders.DataFileBuilder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.Tests.FileBuilders.IndentedFileBuilder;

[UsesVerify]
public class IndentedFileBuilderTests
{
    [Fact]
    public async Task ItShouldWork()
    {
        var file = new IndentedFile("name")
            .WithItems(items =>
            {
                items.WithContent("{");
                items.WithItems(root =>
                {
                    root.WithContent("\"field\": 1,");
                    root.WithContent("\"object\": {");
                    root.WithItems(@object =>
                    {
                        @object.WithContent("\"field\": 1,");
                    });
                    root.WithContent("}");
                });
                items.WithContent("}");
            });
        file.Build();

        await Verifier.Verify(file.ToString());
    }
}