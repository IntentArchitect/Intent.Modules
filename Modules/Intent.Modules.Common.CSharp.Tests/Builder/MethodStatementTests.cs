using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class MethodStatementTests
{
    [Fact]
    public async Task OneLineTernary()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("T", "IIf", m =>
                {
                    m.AddGenericParameter("T", out var t);
                    m.AddParameter("bool", "condition");
                    m.AddParameter(t, "whenTrue");
                    m.AddParameter(t, "whenFalse");
                    m.WithExpressionBody(new CSharpConditionalExpressionStatement("condition", "whenTrue", "whenFalse"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task MultiLineTernary()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("string", "GetContent", m =>
                {
                    m.AddParameter("IServiceProvider", "provider");
                    m.AddParameter("string", "key");
                    m.AddStatement("var contentService = provider.GetService<IContentService>();");
                    m.AddReturn(new CSharpConditionalExpressionStatement(
                        "contentService is not null", 
                        new CSharpConditionalExpressionStatement(
                            "contentService.GetSpecificContent(key) is not null",
                            "contentService.GetSpecificContent(key).GetValue()",
                            "contentService.GetDefaultContent()"), 
                        "contentService.GetDefaultContent()"));
                });
            })
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
                        .AddInvocation("MethodTwo", s => s.OnNewLine())
                        .AddInvocation("MethodThree")
                        .AddInvocation("MethodFour", s => s.OnNewLine())
                    );
                    
                    m.AddStatement(new CSharpObjectInitializerBlock("new Service")
                        .AddInitStatement("State", new CSharpStatement("service").AddInvocation("GetState", c => c.OnNewLine()))
                        .AddInvocation("RegisterService"));

                    m.AddObjectInitializerBlock("new Service", s => s.AddInvocation("DoSomething", x => x.AddArgument("a")));
                });
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task InvocationStatement_MethodChaining_DeeperNesting()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Configure", m =>
                {
                    m.Static();
                    m.AddParameter("Service", "service");

                    m.AddInvocationStatement("services.AddOpenTelemetry", main => main
                        .AddInvocation("ConfigureResource", inv => inv
                            .AddArgument(new CSharpLambdaBlock("res")
                                .WithExpressionBody(new CSharpStatement("res")
                                    .AddInvocation("AddService", inv => inv.AddArgument(@"configuration[""OpenTelemetry:ServiceName""]!").OnNewLine())
                                    .AddInvocation("AddTelemetrySdk", inv => inv.OnNewLine())
                                    .AddInvocation("AddEnvironmentVariableDetector", inv => inv.OnNewLine()).WithoutSemicolon()
                                ))
                            .OnNewLine()));
                });
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
    public async Task TryCatchWithWhenExpressionTest()
    {
        var fileBuilder = new CSharpFile("Namespace", null)
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "Method", method =>
                {
                    method.AddTryBlock();
                    method.AddCatchBlock(@catch =>
                    {
                        @catch.WithExceptionType("Exception");
                        @catch.WithParameterName("e");
                        @catch.WithWhenExpression("e.Message == \"Hello World!\"");
                    });
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task TryCatchFinallyBlocksTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddTryBlock(block => block.AddStatement("DoSomethingRisky();"));
                    method.AddCatchBlock("OutOfMemoryException", "ex", block => block.AddStatement("// What to do?"));
                    method.AddCatchBlock(block => block.AddStatement("// Catch All"));
                    method.AddFinallyBlock(block => block.AddStatement("DoFinallyStuff();"));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task CollectionInitializer()
    {
        var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
            .AddClass("Class", @class =>
            {
                @class.AddMethod("void", "TestMethod", method =>
                {
                    method.AddObjectInitializerBlock("var outer = new Outer", outer =>
                    {
                        outer.AddInitStatement("Details", new CSharpObjectInitializerBlock(null)
                            .AddStatement("\"item\""));
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
}