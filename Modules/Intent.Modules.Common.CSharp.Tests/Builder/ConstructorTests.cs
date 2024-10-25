using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;
public class ConstructorTests
{
    [Fact]
    public async Task ConstructorSingleParameterWithSameLineSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorMultipleParameterWithSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "SameLine");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "name2")
                    .AddParameter("string", "name3")
                    .AddParameter("string", "name4")
                    .AddParameter("string", "name5")
                    .AddParameter("string", "name6")
                    .AddParameter("string", "name7")
                    .AddParameter("string", "name8")
                    .AddParameter("string", "name9")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorSingleParameterWithDefaultSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorMultiShortParameterWithDefaultSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "name2")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorMultiSameLineParameterWithDefaultSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "name2")
                    .AddParameter("string", "name3")
                    .AddParameter("string", "name4")
                    .AddParameter("string", "name5")
                    .AddParameter("string", "name6")
                    .AddParameter("string", "name7")
                    .AddParameter("string", "name8")
                    .AddParameter("string", "name9")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorSingleParameterWithDependsOnLengthSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorTwoParameterWithDependsOnLengthSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "name2")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorMutipleParameterWithDependsOnLengthSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "name2")
                    .AddParameter("string", "name3")
                    .AddParameter("string", "name4")
                    .AddParameter("string", "name5")
                    .AddParameter("string", "name6")
                    .AddParameter("string", "name7")
                    .AddParameter("string", "name8")
                    .AddParameter("string", "name9")
                    .AddParameter("string", "name10")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorNoBaseSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor => ctor
                    .AddParameter("string", "name")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorCallsBaseSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("SameLine", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor =>
                {
                    ctor.AddParameter("string", "name");
                    ctor.CallsBase(bs => bs.AddArgument("name"));
                });

                c.WithBaseType("BaseClass");
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorCallsBaseDependsNewLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("NewLine", "DependsOnLength");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor =>
                {
                    ctor.AddParameter("string", "name");
                    ctor.CallsBase(bs => bs.AddArgument("name"));
                });

                c.WithBaseType("BaseClass");
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorCallsBaseDependsOnLengthShort()
    {
        // setup the style settings
        var settings = new TestStyleSettings("DependsOnLength", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor =>
                {
                    ctor.AddParameter("string", "name");
                    ctor.CallsBase(bs => bs.AddArgument("name"));
                });

                c.WithBaseType("BaseClass");
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ConstructorCallsBaseDependsOnLengthLong()
    {
        // setup the style settings
        var settings = new TestStyleSettings("DependsOnLength", "Default");

        var fileBuilder = new CSharpFile("Namespace", "File", settings)
            .AddClass("Class", c =>
            {
                c.AddConstructor(ctor =>
                {
                    ctor.AddParameter("string", "name");
                    ctor.AddParameter("string", "name2");
                    ctor.AddParameter("string", "name3");
                    ctor.AddParameter("string", "name4");
                    ctor.AddParameter("string", "name5");
                    ctor.CallsBase(bs => bs
                        .AddArgument("name")
                        .AddArgument("name2")
                        .AddArgument("name3")
                        .AddArgument("name4")
                        .AddArgument("name5")
                        );
                });

                c.WithBaseType("BaseClass");
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }
}
