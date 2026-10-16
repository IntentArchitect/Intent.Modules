using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

/// <summary>
/// Statement search is deliberately unchanged: it does not look inside an assignment's right-hand side
/// (<see cref="CSharpAssignmentStatement.Rhs"/>), which ~200 existing FindStatement callers rely on. The
/// items of "options.X = [ ... ]" are reached in two steps instead - find the assignment, then search its
/// Rhs - and, since collection items now know their parent, can then be edited.
/// </summary>
public class StatementSearchTests
{
    [Fact]
    public void SearchDoesNotLookInsideAnAssignmentsRightHandSide()
    {
        var method = Method(m => m.AddStatement(new CSharpAssignmentStatement("options.Items",
            new CSharpCollectionExpression().AddItem("first").AddItem("second"))));

        Assert.Null(method.FindStatement(statement => statement.GetText(string.Empty) == "second"));
    }

    [Fact]
    public void AnItemInAnAssignedCollectionIsReachedThroughTheAssignmentAndCanBeRemoved()
    {
        var method = Method(m => m.AddStatement(new CSharpAssignmentStatement("options.Items",
            new CSharpCollectionExpression().AddItem("first").AddItem("second"))));

        var assignment = method.FindStatement<CSharpAssignmentStatement>(a => a.Lhs.GetText(string.Empty) == "options.Items");
        var item = ((CSharpCollectionExpression)assignment.Rhs).FindStatement(statement => statement.GetText(string.Empty) == "second");
        ((CSharpStatement)item).Remove();

        Assert.Contains("options.Items = [first]", method.ToString());
    }

    [Fact]
    public void SearchResultsWithoutAnAssignmentAreUnchanged()
    {
        var method = Method(m =>
        {
            m.AddStatement("var a = 1;");
            m.AddInvocationStatement("Call", invocation => invocation.AddArgument("x").AddArgument("y"));
            m.AddIfStatement("a > 0", block => block.AddStatement("Inner();"));
        });

        var texts = method.FindStatements(_ => true).Select(statement => statement.GetText(string.Empty).Trim()).ToList();

        Assert.Equal(new[] { "var a = 1;", "Call(x, y);", "if (a > 0)", "x", "y", "Inner();" }, texts.Select(First).ToArray());
    }

    private static string First(string text) => text.Split('\n')[0].Trim();

    private static CSharpClassMethod Method(System.Action<CSharpClassMethod> configure)
    {
        CSharpClassMethod method = null;
        new CSharpFile("Namespace", "File").AddClass("Class", c => c.AddMethod("void", "Configure", m =>
        {
            method = m;
            configure(m);
        })).CompleteBuild();
        return method;
    }
}
