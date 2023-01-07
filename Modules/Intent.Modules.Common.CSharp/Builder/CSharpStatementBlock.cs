using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatementBlock : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon;

    public CSharpStatementBlock() : base(null)
    {
    }

    public List<CSharpStatement> Statements { get; } = new();

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