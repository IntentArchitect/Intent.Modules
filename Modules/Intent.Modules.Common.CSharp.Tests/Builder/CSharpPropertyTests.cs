using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpPropertyTests
{
    [Fact]
    public async Task PropertySetterAccessorTest()
    {
        var settings = new TestStyleSettings("same-line", "depends-on-length", "default");

        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation", settings)
            .AddUsing("System")
            .AddClass("ConcreteClass", @class =>
            {
                @class.AddProperty("string", "NormalProperty");
                @class.AddProperty("string", "PropertyInitSetter", prop => prop.Setter.Init());
                @class.AddProperty("string", "PropertyInternalSetter", prop => prop.Setter.Internal());
                @class.AddProperty("string", "PropertyPrivateSetter", prop => prop.Setter.Private());
                @class.AddProperty("string", "PropertyProtectedSetter", prop => prop.Setter.Protected());
                @class.AddProperty("string", "PropertyProtectedInternalSetter", prop => prop.Setter.ProtectedInternal());
                @class.AddProperty("string", "PropertyPublicSetter", prop => prop.Setter.Public());
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}