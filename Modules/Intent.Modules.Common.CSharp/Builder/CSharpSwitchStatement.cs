using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpSwitchStatement : CSharpStatement, IHasCSharpStatements
{
    private bool _hasDefault = false;

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
        _hasDefault = true;
        Statements.Add(new CSharpSwitchElement("default", null, codeBlock));
        return this;
    }

    public override string GetText(string indentation)
    {
        EnsureDefaultIsLast();
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

    private void EnsureDefaultIsLast()
    {
        if (!_hasDefault) return;
        if (Statements.Last().GetText("").StartsWith("default"))
        {
            return;
        }
        var defaultStatement = Statements.First(s => s.GetText("").StartsWith("default"));
        Statements.Remove(defaultStatement);
        Statements.Add(defaultStatement);    
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}