using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using Xunit;
using VerifyXunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class BuilderTests
{
    [Fact]
    public async Task ClassConstructorTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("TestClass", @class =>
            {
                @class.AddConstructor(ctor => ctor.Static());
                @class.AddConstructor(ctor => ctor.Private());
                @class.AddConstructor(ctor =>
                {
                    ctor.Protected();
                    ctor.AddParameter("string", "field1", param => param.IntroduceField());
                });
                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter("string", "field2", param => param.IntroduceField());
                    ctor.AddParameter("string", "field3", param => param.IntroduceReadonlyField());
                    ctor.AddParameter("string", "property", param => param.IntroduceProperty(p => p.ReadOnly()));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ConstructorCalls()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
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
    public async Task ConcreteClassTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
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

    [Fact]
    public async Task EnumTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
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
    public async Task AllIfVariantStatementsTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
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
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ObjectInitializersTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddObjectInitializerBlock("var obj = new SomeObject", c => c
                        .AddInitStatement("LambdaProp", new CSharpLambdaBlock("x")
                            .AddStatement("return x + 1;"))
                        .AddInitStatement("StringProp", "\"My string\"")
                        .AddInitStatement("IntProp", "5")
                        .WithSemicolon());
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task DictionaryInitializersTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddObjectInitializerBlock("var dict = new Dictionary<string, string>", c => c
                        .AddKeyAndValue(@"""key1""", @"""value 1""")
                        .AddKeyAndValue(@"""key2""", @"""value 2""")
                        .WithSemicolon());
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task UsingBlocksTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddUsingBlock("var scope = service.GetScope()", block => block
                        .AddStatement("scope.Dispose();"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ForEachLoopsTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddForEachStatement("i", "Enumerable.Range(1, 10)", c => c
                        .AddStatement("Console.Write(i);").SeparatedFromPrevious());
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task WhileLoopsTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddStatement("var done = false;");
                    method.AddWhileStatement("!done", c => c
                        .AddStatement("done = true;"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ComplexIfConditionsTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddIfStatement(@"
    !string.IsNullOrWhiteSpace(configuration[""KeyVault:TenantId""]) &&
    !string.IsNullOrWhiteSpace(configuration[""KeyVault:ClientId""]) &&
    !string.IsNullOrWhiteSpace(configuration[""KeyVault:Secret""])", block => block.AddStatement("// If statement body"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task TestStatementBlocks()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddStatement(new CSharpStatementBlock(@"// block expression line 1
    // block expression line 2
    // block expression line 3"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task BasicMethodImplementationTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddStatement(@"var stringVariable = ""String value"";");
                    method.AddStatement(@"var intVariable = 42;");
                    method.AddStatement(@"const string StringConstant = ""Constant Value"";");
                    method.AddStatement(@"const int IntConstant = 77;");
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
    public async Task MethodChainingTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
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
    public async Task InvocationWithMultipleArgsStatementTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
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
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task InvocationWithMethodChainingStatementTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "MethodInvocationTypes", method =>
                {
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
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
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
    public async Task SwitchBreakStatementsTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
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
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task SwitchContinueStatementsTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
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
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task NormalSwitchReturnStatementsTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("string", "SwitchReturnStatements",
                    method =>
                    {
                        method.AddParameter("string", "item");
                        method.AddSwitchStatement("item", swtch => swtch
                            .AddCase(@"""Item1""", cs => cs
                                .WithReturn(@"""Item1"""))
                            .AddCase(@"""Item2""", cs => cs
                                .WithReturn(@"""Item2"""))
                            .AddDefault(cs => cs
                                .WithReturn(@"""Item X""")));
                    });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task NestedSwitchReturnStatementsTest()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
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
    public async Task StaticConfigureStyleFileBuilderTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
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
    public async Task ItShouldNotLoseIndentation()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Method", m =>
                {
                    m.AddStatements(@"
            if (1 == 1 ||
                2 == 2)
            {
                // Nested
            }");
                });
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
    public async Task CollectionExpressionStatementSingleLine()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddAssignmentStatement("var items", new CSharpCollectionExpression("List<(string, string)>"), collection =>
                    {
                        collection.AddItem("(1, 2)");
                        collection.AddItem("(3, 4)");
                    });
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task CollectionExpressionArgumentSingleLine()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddStatement(new CSharpInvocationStatement("SomeMethod"), invocation =>
                    {
                        invocation.AddArgument(new CSharpCollectionExpression(), collection =>
                        {
                            collection.AddItem("(1, 2)");
                            collection.AddItem("(3, 4)");
                        });
                    });
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task CollectionExpressionArgumentMultipleLines()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddStatement(new CSharpInvocationStatement("SomeMethod"), invocation =>
                    {
                        invocation.AddArgument(new CSharpCollectionExpression(), collection =>
                        {
                            for (var i = 0; i < 10; i++)
                            {
                                collection.AddItem($"new SomePrettyLongNamedType({i})");
                            }
                        });
                    });
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
}