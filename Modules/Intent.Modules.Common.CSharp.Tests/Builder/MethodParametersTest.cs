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
    public async Task MethodOneLongParameterDependsOnLength()
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
                    .AddParameter("string", "thisisaveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryveryverylongname")
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

    [Fact]
    public async Task LocalMethodOneParameterDefault()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmParam1")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterShortDefault()
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
                   .AddLocalMethod("void", "LocalMethod", lm => lm
                       .AddParameter("string", "lm1")
                       .AddParameter("string", "lm2")
               ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterLongDefault()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterLongDefaultBoth()
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
                    .AddParameter("string", "mParam2")
                    .AddParameter("string", "mParam3")
                    .AddParameter("string", "mParam4")
                    .AddParameter("string", "mParam5")
                    .AddParameter("string", "mParam6")
                    .AddParameter("string", "mParam7")
                    .AddParameter("string", "mParam8")
                    .AddParameter("string", "mParam9")
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodOneParameterSameLine()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmParam1")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterShortSameLine()
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
                   .AddLocalMethod("void", "LocalMethod", lm => lm
                       .AddParameter("string", "lm1")
                       .AddParameter("string", "lm2")
               ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterLongSameLine()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterLongSameLineBoth()
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
                    .AddParameter("string", "mParam2")
                    .AddParameter("string", "mParam3")
                    .AddParameter("string", "mParam4")
                    .AddParameter("string", "mParam5")
                    .AddParameter("string", "mParam6")
                    .AddParameter("string", "mParam7")
                    .AddParameter("string", "mParam8")
                    .AddParameter("string", "mParam9")
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodOneParameterDependsOnLength()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmParam1")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodOneLongParameterDependsOnLength()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmParammmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterShortDependsOnLength()
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
                   .AddLocalMethod("void", "LocalMethod", lm => lm
                       .AddParameter("string", "lm1")
                       .AddParameter("string", "lm2")
               ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterLongDependsOnLength()
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
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task LocalMethodMultiParameterLongDependsOnLengthBoth()
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
                    .AddParameter("string", "mParam2")
                    .AddParameter("string", "mParam3")
                    .AddParameter("string", "mParam4")
                    .AddParameter("string", "mParam5")
                    .AddParameter("string", "mParam6")
                    .AddParameter("string", "mParam7")
                    .AddParameter("string", "mParam8")
                    .AddParameter("string", "mParam9")
                    .AddLocalMethod("void", "LocalMethod", lm => lm
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                ));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }


    [Fact]
    public async Task InterfaceMethodOneParameterDefault()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("ITest", c =>
            {
                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "mParam1")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodMultiParameterShortDefault()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
             .AddInterface("ITest", c =>
             {

                 c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "p1")
                    .AddParameter("string", "p2")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodMultiParameterLongDefault()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
             .AddInterface("ITest", c =>
             {
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

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodOneParameterSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
             .AddInterface("ITest", c =>
             {
                 c.AddMethod("void", "MyMethod", mth => mth
                   .AddParameter("string", "lmParam1")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodMultiParameterShortSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
             .AddInterface("ITest", c =>
             {
                 c.AddMethod("void", "MyMethod", mth => mth
                       .AddParameter("string", "lm1")
                       .AddParameter("string", "lm2")
               );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodMultiParameterLongSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
             .AddInterface("ITest", c =>
             {
                 c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "lmparam1")
                    .AddParameter("string", "lmparam2")
                    .AddParameter("string", "lmparam3")
                    .AddParameter("string", "lmparam4")
                    .AddParameter("string", "lmparam5")
                    .AddParameter("string", "lmparam6")
                    .AddParameter("string", "lmparam7")
                    .AddParameter("string", "lmparam8")
                    .AddParameter("string", "lmparam9")
                    .AddParameter("string", "lmparam10")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodOneParameterDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("ITest", c =>
            {
                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "mParam1")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodOneLongParameterDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("ITest", c =>
            {
                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "mParam111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodMultiParameterShortDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("ITest", c =>
            {
                c.AddMethod("void", "MyMethod", mth => mth
                    .AddParameter("string", "lm1")
                    .AddParameter("string", "lm2")
               );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }

    [Fact]
    public async Task InterfaceMethodMultiParameterLongDependsOnLength()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddInterface("ITest", c =>
            {
                c.AddMethod("void", "MyMethod", mth => mth
                        .AddParameter("string", "lmparam1")
                        .AddParameter("string", "lmparam2")
                        .AddParameter("string", "lmparam3")
                        .AddParameter("string", "lmparam4")
                        .AddParameter("string", "lmparam5")
                        .AddParameter("string", "lmparam6")
                        .AddParameter("string", "lmparam7")
                        .AddParameter("string", "lmparam8")
                        .AddParameter("string", "lmparam9")
                        .AddParameter("string", "lmparam10")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Interfaces.First().Fields);
        Assert.Empty(fileBuilder.Interfaces.First().Properties);
    }
}
