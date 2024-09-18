using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpSwitchStatement : CSharpStatement, IHasCSharpStatements
{
    public CSharpSwitchStatement(string expression) : base($"switch ({expression})")
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpSwitchStatement AddCase(string value, Action<CSharpSwitchCodeBlock> codeBlock = null)
    {
        Statements.Add(new CSharpSwitchElement("case", value, codeBlock));
        return this;
    }

    public CSharpSwitchStatement AddDefault(Action<CSharpSwitchCodeBlock> codeBlock = null)
    {
        Statements.Add(new CSharpSwitchElement("default", null, codeBlock));
        return this;
    }

    public override string GetText(string indentation)
    {
        var sb = new StringBuilder(128);
        if (Text.Length > 0)
        {
            sb.AppendLine(base.GetText(indentation));
        }

        sb.Append($"{indentation}{RelativeIndentation}{{");
        sb.AppendLine(Statements.ConcatCode($"{indentation}{RelativeIndentation}    "));
        sb.Append($"{indentation}{RelativeIndentation}}}");
        return sb.ToString();
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}