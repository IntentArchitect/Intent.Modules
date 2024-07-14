using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class ConditionalExpressionStatementTests
{
    [Fact]
    public async Task OneLineTernary()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("T", "IIf", m =>
                {
                    m.AddGenericParameter("T", out var t);
                    m.AddParameter("bool", "condition");
                    m.AddParameter(t, "whenTrue");
                    m.AddParameter(t, "whenFalse");
                    m.WithExpressionBody(new CSharpConditionalExpressionStatement("condition", "whenTrue", "whenFalse"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task MultiLineTernary()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("string", "GetContent", m =>
                {
                    m.AddParameter("IServiceProvider", "provider");
                    m.AddParameter("string", "key");
                    m.AddStatement("var contentService = provider.GetService<IContentService>();");
                    m.AddReturn(new CSharpConditionalExpressionStatement(
                        "contentService is not null", 
                        new CSharpConditionalExpressionStatement(
                            "contentService.GetSpecificContent(key) is not null",
                            "contentService.GetSpecificContent(key).GetValue()",
                            "contentService.GetDefaultContent()"), 
                        "contentService.GetDefaultContent()"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
}