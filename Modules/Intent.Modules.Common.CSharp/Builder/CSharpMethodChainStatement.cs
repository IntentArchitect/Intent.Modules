using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpMethodChainStatement : CSharpStatement, IHasCSharpStatements
{
    public CSharpMethodChainStatement(string initialInvocation) : base(initialInvocation)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
    }
    
    public List<CSharpStatement> Statements { get; } = new();

    public CSharpMethodChainStatement AddChainStatement(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        statement.Parent = this;
        Statements.Add(statement);
        statement.BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        statement.AfterSeparator = CSharpCodeSeparatorType.None;
        configure?.Invoke(statement);
        return this;
    }
    
    public override string GetText(string indentation)
    {
        return @$"{indentation}{RelativeIndentation}{Text}{Statements.ConcatCode($"{indentation}{RelativeIndentation}    .")};";
    }
}