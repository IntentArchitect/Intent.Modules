using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpClassTests
{
    [Fact]
    public async Task XmlCommentsOnElements()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.XmlComments.AddStatements("/// Test Class");
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.XmlComments.AddStatements("/// Test Method");
                    method.AddParameter("string", "param", param =>
                    {
                        param.WithXmlDocComment("Test Parameter");
                    });
                });
            })
            .AddClass("CommentedClass", @class =>
            {
                @class.InheritsXmlDocComments();
                @class.AddMethod("void", "Test", m => m.InheritsXmlDocComments(referenceType: "SomeOtherType"));
                @class.AddProperty("string", "Name", p => p.InheritsXmlDocComments());
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ConcreteClassTest()
    {
        var settings = new TestStyleSettings("same-line", "depends-on-length");

        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation", settings)
            .AddUsing("System")
            .AddClass("ConcreteClass", @class =>
            {
                @class.ImplementsInterface("ISomeService");
                @class.WithBaseType("BaseClass");
                @class.AddMethod("void", "Method", method => { method.IsExplicitImplementationFor("ISomeService"); });
                @class.AddMethod("void", "OnTest", method =>
                {
                    method.Override();
                    method.AddStatement("// Do something");
                });
                @class.AddMethod("Task", "MethodAsync", method => { method.Async(); });
                @class.AddMethod("void", "PrivateMethod", method => { method.Private(); });
                @class.AddMethod("void", "ProtectedMethod", method => { method.Protected(); });
                @class.AddField("string", "_test", field => field.Private());
                @class.AddField("string", "_testProtected", field => field.Protected());
                @class.AddField("string", "_testProtectedReadOnly", field => field.ProtectedReadOnly());
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task PrivateStaticReadOnlyShouldBeInCorrectOrder()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddField("object", "Field", f => f.PrivateReadOnly().Static());
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task AsyncOnNestedClassesShouldWork()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddNestedClass("NestedClass", nestedClass =>
                {
                    nestedClass.AddMethod("void", "Method", method => method.Async());
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task PrivateBackingPropertyWithBodyExpressionTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddField("List<object>", "_backingField");

                @class.AddProperty("IReadOnlyCollection<object>", "Property", property =>
                {
                    property.Getter
                        .WithExpressionImplementation("_backingField.AsReadOnlyCollection()")
                        ;

                    property.Setter
                        .WithExpressionImplementation("_backingField = new List<object>(value)")
                        .Private()
                        ;
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task ClassMethodExpressionBodySimpleTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class => { @class.AddMethod("string", "GetDateNow", method => { method.WithExpressionBody("DateTimeOffset.Now"); }); })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ClassMethodExpressionBodyComprehensiveTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("IHostBuilder", "CreateHostBuilder", method =>
                {
                    method.AddParameter("string[]", "args");
                    method.WithExpressionBody(new CSharpMethodChainStatement("Host.CreateDefaultBuilder(args)")
                        .AddChainStatement(new CSharpInvocationStatement("UseSerilog")
                            .WithoutSemicolon()
                            .AddArgument(new CSharpLambdaBlock("(context, services, configuration)")
                                .WithExpressionBody(new CSharpMethodChainStatement("configuration")
                                    .AddChainStatement("ReadFrom.Configuration(context.Configuration)")
                                    .AddChainStatement("ReadFrom.Services(services)")
                                    .AddChainStatement("Enrich.FromLogContext()")
                                    .AddChainStatement("WriteTo.Console()"))))
                        .AddChainStatement(
                            new CSharpInvocationStatement("ConfigureWebHostDefaults")
                                .WithoutSemicolon()
                                .AddArgument(new CSharpLambdaBlock("webBuilder")
                                    .AddStatement("webBuilder.UseStartup<Startup>();"))));
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task InheritanceTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("BaseClass", @class =>
            {
                @class.Abstract();
                @class.AddMethod("void", "ImAbstractOverrideMe", method => method.Abstract());
                @class.AddMethod("void", "ImVirtualOverrideIsOptional", method => method.Virtual().AddStatement("throw new NotImplementedException();"));
            })
            .AddClass("ConcreteClass", @class =>
            {
                @class.WithBaseType("BaseClass");
                @class.AddMethod("void", "ImAbstractOverrideMe", method => method.Override().AddStatement("// Stuff"));
                @class.AddMethod("void", "ImVirtualOverrideIsOptional", method => method.Override().AddStatement("// More Stuff"));
            })
            .AddInterface("ISomeInterface", @interface => { @interface.ImplementsInterfaces("ISomeOtherInterface"); })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task NestedRecordTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("SomeClass", @class =>
            {
                @class.AddNestedRecord("SomeNestedRecord", record =>
                {
                    record.Private();
                    record.AddPrimaryConstructor(ctor => ctor.AddParameter("int", "Identifier"));
                });
            })
            .AddRecord("SomeRecord", record =>
            {
                record.AddProperty("int", "Id");
                record.AddProperty("SomeNestedRecord", "NestedRecord");
                record.AddNestedRecord("SomeOtherNestedRecord", nestedRecord =>
                {
                    nestedRecord.Private();
                    nestedRecord.AddPrimaryConstructor(ctor => ctor.AddParameter("int", "Identifier"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}