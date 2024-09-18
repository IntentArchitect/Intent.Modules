using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatementBlock : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon;

    public CSharpStatementBlock() : base(string.Empty)
    {
    }
    
    public CSharpStatementBlock(string expression) : base(expression ?? string.Empty)
    {
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpStatement SeparatedFromPrevious(bool setting)
    {
        BeforeSeparator = setting ? CSharpCodeSeparatorType.EmptyLines : CSharpCodeSeparatorType.NewLine;
        return this;
    }

    public CSharpStatementBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public override string GetText(string indentation)
    {
        return @$"{(Text.Length > 0 ? GetFormattedMultilineText(indentation) + Environment.NewLine : "")}{indentation}{RelativeIndentation}{{{Statements.ConcatCode($"{indentation}{RelativeIndentation}    ")}
{indentation}{RelativeIndentation}}}{(_withSemicolon ? ";" : "")}";
    }

    // Some expressions may span multiple lines and we still want it to look
    // nicely indented without putting the burden on the user to do it.
    private string GetFormattedMultilineText(string indentation)
    {
        var text = base.GetText(indentation);
        text = text.Replace("\r\n", "\n");
        var lines = text.Split("\n").Select((line, index) => index == 0 ? line : $"{indentation}    " + line.TrimStart());
        var reformatted = string.Join("\n", lines);
        return reformatted;
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => true;
}