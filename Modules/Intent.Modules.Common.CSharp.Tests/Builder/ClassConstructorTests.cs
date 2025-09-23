using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;
public class ClassConstructorTests
{
    [Fact]
    public async Task ConstructorSingleParameterWithSameLineSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

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
    public async Task ClassConstructorTest()
    {
        var settings = new TestStyleSettings("same-line", "default", "default");

        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation", settings)
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
    public async Task ConstructorMultipleParameterWithSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "same-line", "default");

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
        var settings = new TestStyleSettings("same-line", "default", "default");

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
        var settings = new TestStyleSettings("same-line", "default", "default");

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
        var settings = new TestStyleSettings("same-line", "default", "default");

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
    public async Task ConstructorLongParameterWithDefaultSetting()
    {
        // setup the style settings
        var settings = new TestStyleSettings("new-Line", "default", "default");

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
        var settings = new TestStyleSettings("same-line", "depends-on-length", "default");

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
        var settings = new TestStyleSettings("same-line", "depends-on-length", "default");

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
        var settings = new TestStyleSettings("same-line", "depends-on-length", "default");

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
        var settings = new TestStyleSettings("same-line", "default", "default");

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
    public async Task ConstructorCallsBaseSameLine()
    {
        // setup the style settings
        var settings = new TestStyleSettings("same-line", "default", "default");

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
        var settings = new TestStyleSettings("new-line", "depends-on-length", "default");

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
        var settings = new TestStyleSettings("depends-on-length", "default", "default");

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
        var settings = new TestStyleSettings("depends-on-length", "default", "default");

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
