using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
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
}