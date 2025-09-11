using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpFileTests
{
    [Fact]
    public async Task BaseClassTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("BaseClass", @class =>
            {
                @class.Abstract();
                @class.AddMethod("void", "OnTest", method => { method.Virtual(); });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task StaticClassTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("StaticClass", @class =>
            {
                @class.Static();

                @class.AddMethod("void", "StaticMethod", method => method.Static());
                @class.AddProperty("int", "StaticProperty", prop => prop.Static());
                @class.AddField("string", "staticField", field => field.Static().WithAssignment(@"""123"""));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task InterfaceTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Interfaces")
            .AddUsing("System")
            .AddInterface("IInterface", @interface => @interface
                .WithComments("// Comment")
                .AddMethod("void", "Method")
                .AddMethod("void", "Static", method => { method.Static(); })
                .AddProperty("object", "GetterExpression", property =>
                {
                    property.WithoutSetter()
                        .Getter.WithExpressionImplementation("new object()");
                })
            )
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task RecordTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddRecord("TestRecord", rec =>
            {
                rec.AddConstructor();
                rec.AddConstructor(ctor => ctor.AddParameter("string", "param1"));
                rec.AddProperty("string", "Prop1", prop => prop.Init());
                rec.AddMethod("string", "TestMethod", stmt => stmt.AddStatement(@"return """";"));
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task EnumTest()
    {
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation", settings)
            .AddUsing("System")
            .AddEnum("PrivateEnum", e =>
            {
                e.Private();
                e.AddLiteral("Literal1");
            })
            .AddEnum("ProtectedEnum", e =>
            {
                e.Protected();
                e.AddLiteral("Literal1");
            })
            .AddEnum("InternalEnum", e =>
            {
                e.Internal();
                e.AddLiteral("Literal1");
            })
            .AddEnum("InternalProtectedEnum", e =>
            {
                e.InternalProtected();
                e.AddLiteral("Literal1");
            })
            .AddEnum("EnumWithoutValues", e =>
            {
                e.WithComments("// Enum without values");
                e.AddLiteral("Literal1", configure: lit => lit.AddAttribute("[SomeAttribute]"));
                e.AddLiteral("Literal2", configure: lit => lit.WithComments("// Some Comment"));
                e.AddLiteral("Literal3");
            })
            .AddEnum("EnumWithValues", e =>
            {
                e.AddAttribute("[ComprehensiveEnum]");
                e.AddLiteral("Literal1", "1");
                e.AddLiteral("Literal2", "10");
                e.AddLiteral("Literal3", "5000");
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task NamespaceComment()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .WithLeadingTrivia("#pragma warning disable IDE0130 // Namespace does not match folder structure")
            .AddClass("Class")
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}