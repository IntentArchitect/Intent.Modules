using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
public class IntentManagedTests
{
    [Fact]
    public async Task IntentAttributesTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddAssemblyAttribute(CSharpIntentTagModeAttribute.Explicit())
            .AddAssemblyAttribute(CSharpIntentTagModeAttribute.Implicit())
            .AddAssemblyAttribute(CSharpDefaultIntentManagedAttribute.Merge())
            .AddAssemblyAttribute(CSharpDefaultIntentManagedAttribute.Ignore())
            .AddClass("Class", @class =>
            {
                @class.AddAttribute(CSharpIntentManagedAttribute.Fully());
                @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                @class.AddAttribute(CSharpIntentManagedAttribute.Ignore());
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}