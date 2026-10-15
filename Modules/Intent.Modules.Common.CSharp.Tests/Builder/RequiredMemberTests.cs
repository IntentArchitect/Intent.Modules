using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using NSubstitute;
using Shouldly;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class RequiredMemberTests
{
    [Fact]
    public async Task RequiredPropertyAndFieldTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("ConcreteClass", @class =>
            {
                @class.AddField("string", "_requiredField", field => field.Required());
                @class.AddProperty("string", "RequiredProperty", prop => prop.Required());
                @class.AddProperty("string", "RequiredInitProperty", prop => prop.Required().Init());
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Theory]
    [InlineData("10.0", null, false)]
    [InlineData("11.0", null, true)]
    [InlineData("12.0", null, true)]
    [InlineData("latest", null, true)]
    [InlineData("default", "net6.0", false)]
    [InlineData("default", "net7.0", true)]
    public void RequiredIsOnlyEmittedFromCSharp11(string languageVersion, string targetFramework, bool expected)
    {
        var outputTarget = Substitute.For<IOutputTarget>();
        outputTarget.GetTargetPath().Returns([outputTarget]);
        outputTarget.Metadata.Returns(new Dictionary<string, object>
        {
            ["Language Version"] = languageVersion,
            ["Target Frameworks"] = new[] { targetFramework }
        });
        var template = Substitute.For<ICSharpFileBuilderTemplate>();
        template.OutputTarget.Returns(outputTarget);

        var output = new CSharpFile("Testing.Namespace", "RelativeLocation", template)
            .AddClass("ConcreteClass", @class =>
            {
                @class.AddField("string", "_requiredField", field => field.Required());
                @class.AddProperty("string", "RequiredProperty", prop => prop.Required());
            })
            .CompleteBuild()
            .ToString();

        output.ShouldContain("string _requiredField;");
        output.ShouldContain("string RequiredProperty {");
        output.Contains("required string _requiredField;").ShouldBe(expected);
        output.Contains("required string RequiredProperty {").ShouldBe(expected);
    }
}
