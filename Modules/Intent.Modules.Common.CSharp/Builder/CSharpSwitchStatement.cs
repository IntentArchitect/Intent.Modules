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
}

public class CSharpSwitchElement : CSharpStatement
{
    private readonly CSharpSwitchCodeBlock _codeBlock;

    public CSharpSwitchElement(string keyword, string value, Action<CSharpSwitchCodeBlock> codeBlock) : base(string.Empty)
    {
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.None;
        Text = $"{keyword}{(!string.IsNullOrWhiteSpace(value) ? " " + value : string.Empty)}:";
        if (codeBlock is null)
        {
            return;
        }
        _codeBlock = new CSharpSwitchCodeBlock();
        codeBlock(_codeBlock);
    }
    
    public override string GetText(string indentation)
    {
        var sb = new StringBuilder(128);
        if (Text.Length > 0)
        {
            sb.Append(base.GetText(indentation));
        }
        if (_codeBlock is not null)
        {
            sb.Append(_codeBlock.GetText(indentation));
        }
        return sb.ToString();
    }
}

public class CSharpSwitchCodeBlock : CSharpStatement, IHasCSharpStatements
{
    private EndType _endType = EndType.None;
    private CSharpStatement _endTypeStatement;

    public CSharpSwitchCodeBlock() : base(string.Empty)
    {
        BeforeSeparator = CSharpCodeSeparatorType.None;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpSwitchCodeBlock AddStatement(string statement)
    {
        Statements.Add(statement);
        return this;
    }
    
    public CSharpSwitchCodeBlock WithBreak()
    {
        _endType = EndType.Break;
        return this;
    }
    
    public CSharpSwitchCodeBlock WithContinue()
    {
        _endType = EndType.Continue;
        return this;
    }
    
    public CSharpSwitchCodeBlock WithReturn(CSharpStatement statement)
    {
        _endType = EndType.Return;
        _endTypeStatement = statement;
        return this;
    }

    private enum EndType
    {
        None = 0,
        Break,
        Continue,
        Return
    }

    public override string GetText(string indentation)
    {
        var sb = new StringBuilder(128);
        if (Text.Length > 0)
        {
            sb.Append(base.GetText(indentation));
        }

        sb.Append(Statements.ConcatCode($"{indentation}{RelativeIndentation}    "));
        switch (_endType)
        {
            case EndType.None:
                break;
            case EndType.Break:
                sb.AppendLine();
                sb.Append($"{indentation}{RelativeIndentation}    break;");
                break;
            case EndType.Continue:
                sb.AppendLine();
                sb.Append($"{indentation}{RelativeIndentation}    continue;");
                break;
            case EndType.Return:
                sb.AppendLine();
                sb.Append($"{indentation}{RelativeIndentation}    return {_endTypeStatement.GetText(string.Empty)};");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return sb.ToString();
    }
}