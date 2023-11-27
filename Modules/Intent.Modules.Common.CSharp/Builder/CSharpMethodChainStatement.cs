using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpMethodChainStatement : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon = true;
    
    public CSharpMethodChainStatement(string initialInvocation) : base(initialInvocation)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpMethodChainStatement AddChainStatement(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        statement.Parent = this;
        Statements.Add(statement);
        statement.BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        statement.AfterSeparator = CSharpCodeSeparatorType.None;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpMethodChainStatement WithoutSemicolon()
    {
        _withSemicolon = false;
        return this;
    }
    
    public override string GetText(string indentation)
    {
        return @$"{indentation}{RelativeIndentation}{Text}{Statements.ConcatCode($"{indentation}{RelativeIndentation}    ", s => $".{s}")}{(_withSemicolon ? ";" : "")}";
    }
}