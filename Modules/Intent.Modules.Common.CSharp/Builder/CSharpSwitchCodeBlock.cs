using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

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