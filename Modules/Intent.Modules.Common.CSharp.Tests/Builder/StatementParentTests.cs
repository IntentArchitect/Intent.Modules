using Intent.Modules.Common.CSharp.Builder;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

/// <summary>
/// Collection items and switch-case statements know their parent, as invocation arguments always
/// have, so they can be removed, replaced or have statements inserted beside them.
/// </summary>
public class StatementParentTests
{
    [Fact]
    public void ACollectionItemCanBeRemoved()
    {
        CSharpStatement first = null;
        var collection = new CSharpCollectionExpression()
            .AddItem("a", item => first = item)
            .AddItem("b");

        first.Remove();

        Assert.Equal("[b]", collection.GetText(string.Empty));
    }

    [Fact]
    public void StatementsCanBeInsertedAboveAndBelowACollectionItem()
    {
        CSharpStatement middle = null;
        var collection = new CSharpCollectionExpression()
            .AddItem("a")
            .AddItem("m", item => middle = item)
            .AddItem("z");

        middle.InsertAbove(new CSharpStatement("before"));
        middle.InsertBelow(new CSharpStatement("after"));

        Assert.Equal("[a, before, m, after, z]", collection.GetText(string.Empty));
    }

    [Fact]
    public void AddInvocationOnACollectionItemReplacesItWithoutASemicolon()
    {
        var collection = new CSharpCollectionExpression()
            .AddItem("services", item => item.AddInvocation("Build"));

        Assert.Equal("[services.Build()]", collection.GetText(string.Empty));
    }

    [Fact]
    public void AddInvocationOnACollectionItemCanStillAskForASemicolon()
    {
        var collection = new CSharpCollectionExpression()
            .AddItem("services", item => item.AddInvocation("Build", invocation => invocation.WithSemicolon()));

        Assert.Equal("[services.Build();]", collection.GetText(string.Empty));
    }

    [Fact]
    public void ACollectionAsAnArgumentRendersAsBefore()
    {
        var invocation = new CSharpInvocationStatement("Configure")
            .AddArgument(new CSharpCollectionExpression().AddItem("1").AddItem("2"));

        Assert.Equal("Configure([1, 2]);", invocation.GetText(string.Empty));
    }

    [Fact]
    public void ACollectionAsALambdaExpressionBodyRendersAsBefore()
    {
        var lambda = new CSharpLambdaBlock("()")
            .WithExpressionBody(new CSharpCollectionExpression().AddItem("1").AddItem("2"));

        Assert.Equal("() => [1, 2]", lambda.GetText(string.Empty));
    }

    [Fact]
    public void ANestedCollectionRendersAsBefore()
    {
        var collection = new CSharpCollectionExpression()
            .AddItem(new CSharpCollectionExpression().AddItem("1"))
            .AddItem("2");

        Assert.Equal("[[1], 2]", collection.GetText(string.Empty));
    }

    [Fact]
    public void ASwitchCaseStatementCanBeRemovedOrHaveStatementsInsertedBesideIt()
    {
        CSharpSwitchCodeBlock caseBlock = null;
        var switchStatement = new CSharpSwitchStatement("value")
            .AddCase("1", block => caseBlock = block.AddStatement("Keep();").AddStatement("Drop();").WithBreak());

        caseBlock.Statements[1].Remove();
        caseBlock.Statements[0].InsertAbove(new CSharpStatement("First();"));

        var text = switchStatement.GetText(string.Empty);
        Assert.Contains("First();", text);
        Assert.Contains("Keep();", text);
        Assert.DoesNotContain("Drop();", text);
        Assert.True(text.IndexOf("First();") < text.IndexOf("Keep();"));
    }
}
