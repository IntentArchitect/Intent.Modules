using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using Xunit;
using VerifyXunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
public class BuilderTests
{
    [Fact]
    public async Task BasicClassBuilderTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .AddClass("Vehicle", @class =>
            {
                @class.AddConstructor(ctor => ctor
                        .AddParameter("string", "make")
                        .AddParameter("string", "model")
                        .AddParameter("int", "year", param => param.WithDefaultValue("2023"))
                        .AddStatement("Make = make;")
                        .AddStatement("Model = model;")
                        .AddStatement("Year = year;"))
                    .AddProperty("string", "Make")
                    .AddProperty("string", "Model")
                    .AddProperty("int", "Year")
                    .AddMethod("void", "StartEngine", method => method
                        .AddStatement(@"Console.WriteLine(""The engine is running."");"))
                    .AddMethod("void", "Drive", method => method
                        .WithComments("// Method with parameters")
                        .AddStatement(@"Console.WriteLine($""The {Make} {Model} drove {distance} miles at {speed} mph."");")
                        .AddParameter("int", "distance")
                        .AddParameter("int", "speed"))
                    .AddMethod("int", "CalculateAge", method => method
                        .WithComments("// Method with return value")
                        .AddStatement("int currentYear = DateTime.Now.Year;")
                        .AddStatement("return currentYear - Year;"))
                    .AddMethod("string", "ToString", method => method
                        .WithComments("// Method that overrides a virtual method")
                        .Override()
                        .AddStatement(@"return $""{Make} {Model} ({Year})"";"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task StaticConfigureStyleFileBuilderTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddUsing("System")
            .AddUsing("Azure")
            .AddUsing("Azure.Messaging.EventGrid")
            .AddUsing("Azure.Messaging.ServiceBus")
            .AddUsing("Microsoft.EntityFrameworkCore")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass("DependencyInjection", @class =>
            {
                @class.Static()
                    .AddMethod("IServiceCollection", "AddInfrastructure", method => method
                        .Static()
                        .AddParameter("IServiceCollection", "services", param => param.WithThisModifier())
                        .AddParameter("IConfiguration", "configuration")
                        .AddStatement(new CSharpInvocationStatement("services.AddDbContext<ApplicationDbContext>")
                            .AddArgument(new CSharpLambdaBlock("(sp, options)")
                                .AddStatement(@"options.UseInMemoryDatabase(""DefaultConnection"");")
                                .AddStatement(@"options.UseLazyLoadingProxies();"))
                            .WithArgumentsOnNewLines())
                        .AddStatement(new CSharpInvocationStatement(@"services.AddScoped<IUnitOfWork>")
                            .AddArgument(new CSharpLambdaBlock("provider")
                                .WithExpressionBody(@"provider.GetService<ApplicationDbContext>()"))));

            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task StatementBlocks()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddParameter("int", "value");

                    method.AddIfStatement("value == 0", c => c
                        .AddStatement("throw new InvalidArgumentException();"));
                    method.AddElseIfStatement("value == 1", c => c
                        .AddStatement("return 1;"));
                    method.AddElseStatement(c => c
                        .AddStatement("return 2;"));

                    method.AddStatement("// Object Init", s => s.SeparatedFromPrevious())
                        .AddObjectInitializerBlock("var obj = new SomeObject", c => c
                            .AddInitStatement("LambdaProp", new CSharpLambdaBlock("x")
                                .AddStatement("return x + 1;"))
                            .AddInitStatement("StringProp", "\"My string\"")
                            .WithSemicolon());

                    method.AddObjectInitializerBlock("var dict = new Dictionary<string, string>", c => c
                        .AddKeyAndValue(@"""key1""", @"""value 1""")
                        .AddKeyAndValue(@"""key2""", @"""value 2""")
                        .WithSemicolon());

                    method.AddUsingBlock("var scope = service.GetScope()", block => block
                        .AddStatement("scope.Dispose();"));

                    method.AddStatement("// New Scope")
                        .AddStatement(new CSharpStatementBlock());

                    method.AddForEachStatement("i", "Enumerable.Range(1, 10)", c => c
                        .AddStatement("Console.Write(i);").SeparatedFromPrevious());

                    method.AddTryBlock(block => block.AddStatement("DoSomethingRisky();"));
                    method.AddCatchBlock("OutOfMemoryException", "ex", block => block.AddStatement("// What to do?"));
                    method.AddCatchBlock(block => block.AddStatement("// Catch All"));
                    method.AddFinallyBlock(block => block.AddStatement("DoFinallyStuff();"));

                    method.AddIfStatement(@"
    !string.IsNullOrWhiteSpace(configuration[""KeyVault:TenantId""]) &&
	!string.IsNullOrWhiteSpace(configuration[""KeyVault:ClientId""]) &&
	!string.IsNullOrWhiteSpace(configuration[""KeyVault:Secret""])", block => block.AddStatement("// If statement body"));

                    method.AddStatement(new CSharpStatementBlock(@"// block expression line 1
// block expression line 2
// block expression line 3"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task StaticVariants()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddClass("StaticClass", @class =>
            {
                @class.Static();
                @class.AddConstructor(ctor => ctor.Static());
                @class.AddMethod("void", "StaticMethod", method => method.Static());
                @class.AddProperty("int", "StaticProperty", prop => prop.Static());
                @class.AddField("string", "_staticField", field => field.Static().WithAssignment(@"""123"""));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task Inheritance()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
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
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task BaseTypeWithGenericParameters()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddClass("Class", @class =>
            {
                @class.WithBaseType("BaseType", new[] { "GenericTypeParameter1", "GenericTypeParameter2" });
                @class.AddConstructor();
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ConstructorCalls()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddUsing("System")
            .AddClass("ConcreteClass", @class =>
            {
                @class.WithBaseType("MyBaseClass");
                @class.AddConstructor(ctor => ctor
                    .CallsBase());
                @class.AddConstructor(ctor => ctor
                    .AddParameter("bool", "enabled")
                    .CallsThis());
                @class.AddConstructor(ctor => ctor
                    .AddParameter("string", "value")
                    .CallsThis(t => t.AddArgument("value").AddArgument("1")));
                @class.AddConstructor(ctor => ctor
                    .AddParameter("string", "value")
                    .AddParameter("int", "otherValue")
                    .CallsBase(b => b.AddArgument("value").AddArgument("otherValue")));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task MethodExplicitlyImplementingForInterface()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Method", method =>
                {
                    method.IsExplicitImplementationFor("IInterface");
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task MethodChainingTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddUsing("System")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "MethodChainTest", method =>
                {
                    method.AddMethodChainStatement("services.AddOpenTelemetry()", main => main
                        .AddChainStatement(new CSharpInvocationStatement("ConfigureResource")
                            .AddArgument(new CSharpLambdaBlock("res")
                                .WithExpressionBody(new CSharpMethodChainStatement("res")
                                    .WithoutSemicolon()
                                    .AddChainStatement($@"AddService(""TestService"")")
                                    .AddChainStatement("AddTelemetrySdk()")
                                    .AddChainStatement("AddEnvironmentVariableDetector()")))
                            .WithoutSemicolon())
                        .AddChainStatement(new CSharpInvocationStatement("WithTracing")
                            .AddArgument(new CSharpLambdaBlock("trace")
                                .WithExpressionBody(new CSharpMethodChainStatement("trace")
                                    .WithoutSemicolon()
                                    .AddChainStatement("AddAspNetCoreInstrumentation()")))
                            .WithoutSemicolon())
                        .AddMetadata("telemetry-config", true));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task PrivatePropertyTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
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
    public async Task InterfaceTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Interfaces")
            .AddUsing("System")
            .AddInterface("IInterface", @interface => @interface
                .WithComments("// Comment")
                .AddMethod("void", "Method")
                .AddMethod("void", "Static", method =>
                {
                    method.Static();
                })
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
    public async Task ClassMethodExpressionBodyTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("string", "GetDateNow", method =>
                {
                    method.WithExpressionBody("DateTimeOffset.Now");
                });
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
    public async Task InvocationStatementTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "MethodInvocationTypes", method =>
                {
                    method.AddInvocationStatement("TestMethodNoArgs");
                    method.AddInvocationStatement("TestMethodOneArg", m => m.AddArgument("one"));
                    method.AddInvocationStatement("TestMethodTwoArgs", m => m.AddArgument("one").AddArgument("two"));
                    method.AddInvocationStatement("TestMethodMultilineWithOneArg", m => m
                        .WithArgumentsOnNewLines()
                        .AddArgument("one"));
                    method.AddInvocationStatement("TestMethodMultilineArgs", m => m
                        .WithArgumentsOnNewLines()
                        .AddArgument("one")
                        .AddArgument("two")
                        .AddArgument("three"));
                    method.AddInvocationStatement("TestMethodWithMethodChainingArg", stmt => stmt
                        .AddArgument(new CSharpMethodChainStatement("fluentBuilder")
                            .AddChainStatement("FluentOpOne()")
                            .AddChainStatement("FluentOpTwo()")
                            .WithoutSemicolon()));
                    method.AddStatement(new CSharpMethodChainStatement(@"services.ConfigureComponent()")
                        .AddChainStatement(new CSharpInvocationStatement("ConfigureFeatures")
                            .WithoutSemicolon()
                            .AddArgument(@"""FeatureSet1""")
                            .AddArgument(new CSharpLambdaBlock("conf")
                                .WithExpressionBody(new CSharpMethodChainStatement("conf")
                                    .WithoutSemicolon()
                                    .AddChainStatement("SwitchFeatureOne(true)")
                                    .AddChainStatement("SwitchFeatureTwo(false)")))));
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task MethodParametersTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "NoParamMethod");
                @class.AddMethod("void", "SingleParamMethod", method => method
                    .AddParameter("string", "parm1")
                    .AddStatement("// Expect parameters on same line"));
                @class.AddMethod("void", "DoubleParamsMethod", method => method
                    .AddParameter("string", "parm1")
                    .AddParameter("string", "parm2")
                    .AddStatement("// Expect parameters on same line"));
                @class.AddMethod("void", "TripleParamsMethod", method => method
                    .AddParameter("string", "parm1")
                    .AddParameter("string", "parm2")
                    .AddParameter("string", "parm3")
                    .AddStatement("// Expect parameters on same line"));
                @class.AddMethod("void", "LongAndManyParamsMethod", method => method
                    .AddParameter("string", "firstParameter")
                    .AddParameter("string", "secondParameter")
                    .AddParameter("string", "thirdParameter")
                    .AddParameter("string", "fourthParameter")
                    .AddStatement("// Expect parameters to span over multiple lines"));
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task SwitchStatementTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "SwitchBreakStatements",
                    method =>
                    {
                        method.AddParameter("Exception", "exception");
                        method.AddSwitchStatement("exception", stmt => stmt
                            .AddCase("ArgumentNullException")
                            .AddCase("NullReferenceException", block => block
                                .AddStatement(@"Console.WriteLine(""Null detected"");")
                                .WithBreak())
                            .AddCase("OutOfMemoryException", block => block
                                .AddStatement(@"Console.WriteLine(""No memory"");")
                                .WithBreak())
                            .AddDefault(block => block
                                .AddStatement(@"Console.WriteLine(exception.GetType().Name);")
                                .WithBreak()));
                    });
                @class.AddMethod("void", "SwitchContinueStatements",
                    method =>
                    {
                        method.AddParameter("IEnumerable<string>", "collection");
                        method.AddForEachStatement("item", "collection", stmt => stmt
                            .AddSwitchStatement("item", swtch => swtch
                                .AddCase(@"""Item1""", cs => cs.AddStatement(@"Console.WriteLine(""Item1"");")
                                    .WithContinue())
                                .AddCase(@"""Item2""", cs => cs.AddStatement(@"Console.WriteLine(""Item2"");")
                                    .WithContinue())));
                        method.AddStatement(@"Console.WriteLine(""Item X"");");
                    });
                @class.AddMethod("string", "SwitchReturnStatements",
                    method =>
                    {
                        method.AddParameter("IEnumerable<string>", "collection");
                        method.AddForEachStatement("item", "collection", stmt => stmt
                            .AddSwitchStatement("item", swtch => swtch
                                .AddCase(@"""Item1""", cs => cs
                                    .WithReturn(@"""Item1"""))
                                .AddCase(@"""Item2""", cs => cs
                                    .WithReturn(@"""Item2"""))));
                        method.AddStatement(@"return ""Item X"";");
                    });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task GenericsTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "Class")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddGenericParameter("T", out var t);
                @class.AddMethod("void", "GenericMethod", method => method
                    .AddGenericParameter(t)
                    .AddGenericParameter("U", out var u)
                    .AddGenericTypeConstraint(u, c => c.AddType("class")));
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}