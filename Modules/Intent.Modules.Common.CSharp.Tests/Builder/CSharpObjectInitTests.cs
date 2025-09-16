using Intent.Modules.Common.CSharp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;
public class CSharpObjectInitTests
{
    [Fact]
    public async Task NoChangeObjectInit()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("MyClass", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.AddObjectInitStatement("var x", "123;");
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.Equal("var x", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.LeftHandSide);
        Assert.Equal("123;", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.RightHandSide.ToString());
    }

    [Fact]
    public async Task LhsChangedObjectInit()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("MyClass", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.AddObjectInitStatement("var x", "123;");
                });
            })
            .CompleteBuild();

        (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.WithLeftHandSide("var y");

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.Equal("var y", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.LeftHandSide);
        Assert.Equal("123;", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.RightHandSide.ToString());
    }

    [Fact]
    public async Task RhsChangedObjectInit()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("MyClass", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.AddObjectInitStatement("var x", "123;");
                });
            })
            .CompleteBuild();

        (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.WithRightHandSide("456;");

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.Equal("var x", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.LeftHandSide);
        Assert.Equal("456;", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.RightHandSide.ToString());
    }

    [Fact]
    public async Task LhsAndRhsChangedObjectInit()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("MyClass", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.AddObjectInitStatement("var x", "456;");
                });
            })
            .CompleteBuild();

        (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.WithLeftHandSide("var y").WithRightHandSide("789;");

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.Equal("var y", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.LeftHandSide);
        Assert.Equal("789;", (fileBuilder.Classes.First().Methods.First().Statements.First() as CSharpObjectInitStatement)!.RightHandSide.ToString());
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

}
