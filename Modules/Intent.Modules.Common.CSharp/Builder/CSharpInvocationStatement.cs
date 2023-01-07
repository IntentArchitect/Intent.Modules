using System;
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

public class CSharpInvocationStatement : CSharpStatement, IHasCSharpStatements
{
    private CSharpCodeSeparatorType _defaultArgumentSeparator = CSharpCodeSeparatorType.None;

    public CSharpInvocationStatement(string invocation) : base(invocation)
    {
    }

    public List<CSharpStatement> Statements { get; } = new();

    public CSharpInvocationStatement AddArgument(CSharpStatement argument, Action<CSharpStatement> configure = null)
    {
        argument.Parent = this;
        Statements.Add(argument);
        argument.BeforeSeparator = _defaultArgumentSeparator;
        argument.AfterSeparator = CSharpCodeSeparatorType.None;
        configure?.Invoke(argument);
        return this;
    }

    public CSharpInvocationStatement WithArgumentsOnNewLines()
    {
        foreach (var argument in Statements)
        {
            argument.BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        }

        _defaultArgumentSeparator = CSharpCodeSeparatorType.NewLine;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{Text}({GetArgumentsText(indentation)});";
    }

    private string GetArgumentsText(string indentation)
    {
        return Statements.JoinCode(",", $"{indentation}    ");
    }
}