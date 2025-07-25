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
        var settings = new TestStyleSettings("same-line", "same-line");

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
        var settings = new TestStyleSettings("same-line", "same-line");

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
        var settings = new TestStyleSettings("same-line", "same-line");

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
        var settings = new TestStyleSettings("same-line", "same-line");

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
        var settings = new TestStyleSettings("same-line", "same-line");

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
        var settings = new TestStyleSettings("same-line", "same-line");

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
}
