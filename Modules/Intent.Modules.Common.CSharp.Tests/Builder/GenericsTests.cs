using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class GenericsTests
{
    [Fact]
    public async Task GenericsTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddGenericParameter("T", out var t);
                @class.AddMethod("void", "GenericMethod", method => method
                    .AddGenericParameter(t)
                    .AddGenericParameter("U", out var u)
                    .AddGenericTypeConstraint(u, c => c.AddType("class")));
            })
            .AddInterface("Interface", @interface =>
            {
                @interface.AddGenericParameter("T", out var t);
                @interface.AddMethod("void", "GenericMethod", method => method
                    .AddGenericParameter(t)
                    .AddGenericParameter("U", out var u)
                    .AddGenericTypeConstraint(u, c => c.AddType("class")));
            })
            .AddClass("DerivedClass", @class =>
            {
                @class.WithBaseType("BaseType", new[] { "GenericTypeParameter1", "GenericTypeParameter2" });
                @class.AddConstructor();
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}