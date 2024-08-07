using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class ChainedInvocationTests
{
    [Fact]
    public async Task InvocationStatement_MethodChaining()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Configure", m =>
                {
                    m.Static();
                    m.AddParameter("Service", "service");
                    m.AddStatement(new CSharpStatement("service")
                        .AddInvocation("MethodOne")
                        .AddInvocation("MethodTwo", s => s.SeparateOnNewLine())
                        .AddInvocation("MethodThree")
                        .AddInvocation("MethodFour", s => s.SeparateOnNewLine())
                    );
                    
                    m.AddStatement(new CSharpObjectInitializerBlock("new Service")
                        .AddInitStatement("State", new CSharpStatement("service").AddInvocation("GetState", c => c.SeparateOnNewLine()))
                        .AddInvocation("RegisterService"));
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}