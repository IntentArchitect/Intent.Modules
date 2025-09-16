using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;
using NSubstitute;
using NSubstitute.Extensions;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpMethodTests
{
    [Fact]
    public async Task MethodStaticModifierSet()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.Static();
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodStaticModifierNotSet()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.Static();
                });
            })
            .CompleteBuild();

        fileBuilder.Classes.First().Methods.First().WithoutMethodModifier();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodAbstractModifierSet()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.Abstract();
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.True(fileBuilder.Classes.First().Methods.First().IsAbstract);
        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodAbstractModifierNotSet()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.Abstract();
                });
            })
            .CompleteBuild();

        fileBuilder.Classes.First().Methods.First().WithoutMethodModifier();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);

        Assert.False(fileBuilder.Classes.First().Methods.First().IsAbstract);
        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodStaticModifierSet()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("MyInterface", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.Static();
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task InterfaceMethodStaticModifierNotSet()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("MyInterface", c =>
            {
                c.AddMethod("void", "TestMethod", m =>
                {
                    m.Static();
                });
            })
            .CompleteBuild();

        fileBuilder.Interfaces.First().Methods.First().WithoutMethodModifier();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task MultipleAddOptionalCancellationTokenParameter_Interface()
    {
        var template = Substitute.For<ICSharpFileBuilderTemplate>();
        template.UseType(Arg.Any<string>()).Returns("CancellationToken");

        var fileBuilder = new CSharpFile("Namespace", "File", template)
            .AddInterface("IInterface", c =>
            {
                c.AddMethod("void", "Method", m =>
                {
                    m.AddOptionalCancellationTokenParameter();
                    m.AddOptionalCancellationTokenParameter();
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task MultipleAddOptionalCancellationTokenParameter_Class()
    {
        var template = Substitute.For<ICSharpFileBuilderTemplate>();
        template.UseType(Arg.Any<string>()).Returns("CancellationToken");

        var fileBuilder = new CSharpFile("Namespace", "File", template)
            .AddClass("Class", c =>
            {
                c.AddMethod("void", "Method", m =>
                {
                    m.AddOptionalCancellationTokenParameter();
                    m.AddOptionalCancellationTokenParameter();
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
    }

    [Fact]
    public async Task MultipleAddOptionalCancellationTokenParameter_Local()
    {
        var template = Substitute.For<ICSharpFileBuilderTemplate>();
        template.UseType(Arg.Any<string>()).Returns("CancellationToken");

        var fileBuilder = new CSharpFile("Namespace", "File", template)
            .AddClass("Class", c =>
            {
                c.AddMethod("void", "Method", m =>
                {
                    m.AddLocalMethod("void", "LocalMethod", lm =>
                    {
                        lm.AddOptionalCancellationTokenParameter();
                        lm.AddOptionalCancellationTokenParameter();
                    });
                });
            })
            .CompleteBuild();

        var result = fileBuilder.ToString();
        await Verifier.Verify(result);
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
}
