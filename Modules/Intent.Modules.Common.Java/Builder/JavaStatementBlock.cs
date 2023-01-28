using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Builder;

public class JavaStatementBlock: JavaStatement, IHasJavaStatements
{
    private bool _withSemicolon;

    public JavaStatementBlock(string expression = null) : base(expression ?? string.Empty)
    {
    }

    public IList<JavaStatement> Statements { get; } = new List<JavaStatement>();

    public JavaStatementBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public override string GetText(string indentation)
    {
        return @$"{base.GetText(indentation)}{(Text != string.Empty ? " " : "")}{{{Statements.ConcatCode($"{indentation}{RelativeIndentation}    ")}
{indentation}{RelativeIndentation}}}{(_withSemicolon ? ";" : "")}";
    }
}