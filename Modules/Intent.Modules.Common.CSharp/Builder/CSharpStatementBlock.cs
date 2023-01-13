using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatementBlock : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon = false;

    public CSharpStatementBlock() : base(null)
    {
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpStatementBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public override string GetText(string indentation)
    {
        return @$"{indentation}{RelativeIndentation}{{{Statements.ConcatCode($"{indentation}{RelativeIndentation}    ")}
{indentation}{RelativeIndentation}}}{(_withSemicolon ? ";" : "")}";
    }
}