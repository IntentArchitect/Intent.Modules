using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpLambdaBlock : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon = false;

    public CSharpLambdaBlock(string invocation) : base(invocation)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpLambdaBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public override string GetText(string indentation)
    {
        return @$"{base.GetText(indentation)} =>
{indentation}{RelativeIndentation}{{{Statements.ConcatCode($"{indentation}{RelativeIndentation}    ")}
{indentation}{RelativeIndentation}}}{(_withSemicolon ? ";" : "")}";
    }
}