using Intent.Modules.Common.CSharp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;
public class MethodParametersTest
{
    [Fact]
    public async Task MethodOneParameterDefault()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "mParam1")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodMultiParameterShortDefault()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "p1")
                    .AddParameter("string", "p2")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodMultiParameterLongDefault()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "param1")
                    .AddParameter("string", "param2")
                    .AddParameter("string", "param3")
                    .AddParameter("string", "param4")
                    .AddParameter("string", "param5")
                    .AddParameter("string", "param6")
                    .AddParameter("string", "param7")
                    .AddParameter("string", "param8")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodOneParameterSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "mParam1")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodMultiParameterShortSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "p1")
                    .AddParameter("string", "p2")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodMultiParameterLongSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "param1")
                    .AddParameter("string", "param2")
                    .AddParameter("string", "param3")
                    .AddParameter("string", "param4")
                    .AddParameter("string", "param5")
                    .AddParameter("string", "param6")
                    .AddParameter("string", "param7")
                    .AddParameter("string", "param8")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodOneParameterDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "mParam1")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodMultiParameterShortDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "p1")
                    .AddParameter("string", "p2")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task MethodMultiParameterLongDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );

                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "param1")
                    .AddParameter("string", "param2")
                    .AddParameter("string", "param3")
                    .AddParameter("string", "param4")
                    .AddParameter("string", "param5")
                    .AddParameter("string", "param6")
                    .AddParameter("string", "param7")
                    .AddParameter("string", "param8")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }
}
