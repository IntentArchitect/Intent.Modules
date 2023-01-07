using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptStatementBlock : TypescriptStatement, IHasTypescriptStatements
{
    private bool _withSemicolon;

    public TypescriptStatementBlock() : base(null)
    {
    }

    public List<TypescriptStatement> Statements { get; } = new();

    public TypescriptStatementBlock WithSemicolon()
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