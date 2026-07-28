using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class BuilderTopLevelStatementsTests
{
    [Fact]
    public async Task AloneShouldWork()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddTopLevelStatements(tls =>
            {
                tls.AddStatement("Console.WriteLine(\"Hello world!\");");
                tls.AddLocalMethod("Task", "LocalMethod", localMethod =>
                {
                    localMethod.AddParameter("object", "parameter");
                    localMethod.Static().Async();
                    localMethod.AddStatement("var variable = new object();");
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task WithTypeDeclarationsShouldWork()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddTopLevelStatements(tls =>
            {
                tls.AddStatement("Console.WriteLine(\"Hello world!\");");
                tls.AddLocalMethod("Task", "LocalMethod", localMethod =>
                {
                    localMethod.AddParameter("object", "parameter");
                    localMethod.Static().Async();
                    localMethod.AddStatement("var variable = new object();");
                });
            })
            .AddClass("Class")
            .AddRecord("Record")
            .AddInterface("IInterface")
            .AddEnum("Enum")
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task WithLocalMethodAttributesShouldWork()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddTopLevelStatements(tls =>
            {
                tls.AddStatement("Console.WriteLine(\"Hello world!\");");
                tls.AddLocalMethod("void", "AddCustomRegistrations", localMethod =>
                {
                    localMethod.Static();
                    localMethod.AddAttribute("Obsolete");
                    localMethod.AddAttribute("Conditional", attribute => attribute.AddArgument("\"DEBUG\""));
                    localMethod.AddAttribute(new CSharpAttribute("[Description(\"Local\")]"));
                    localMethod.AddParameter("IDistributedApplicationBuilder", "builder");
                    localMethod.AddStatement("// Add your custom registrations here");
                });
                tls.AddInvocationStatement("Nested", inv => inv.AddArgument(new CSharpLambdaBlock("x")
                    .AddLocalMethod("int", "InnerMethod", innerMethod =>
                    {
                        innerMethod.AddAttribute("Obsolete");
                        innerMethod.WithExpressionBody(new CSharpStatement("1"));
                    })));
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}