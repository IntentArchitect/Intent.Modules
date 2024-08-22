using System.Threading.Tasks;
using Intent.Modules.Common.Java.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class ChainedInvocationTests
{
    [Fact]
    public async Task InvocationStatement_MethodChaining()
    {
        var fileBuilder = new JavaFile("test.namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "configure", m =>
                {
                    m.Static();
                    m.AddParameter("Service", "service");
                    m.AddStatement(new JavaStatement("service")
                        .AddInvocation("methodOne")
                        .AddInvocation("methodTwo", s => s.OnNewLine())
                        .AddInvocation("methodThree")
                        .AddInvocation("methodFour", s => s.OnNewLine())
                    );
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}