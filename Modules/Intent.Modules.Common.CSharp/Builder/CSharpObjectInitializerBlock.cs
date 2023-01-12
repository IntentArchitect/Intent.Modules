using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitializerBlock : CSharpStatementBlock
{
    public CSharpObjectInitializerBlock(string invocation) : base()
    {
        Text = invocation;
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public new CSharpObjectInitializerBlock WithSemicolon()
    {
        base.WithSemicolon();
        return this;
    }

    public CSharpObjectInitializerBlock AddInitStatement(string lhs, CSharpStatement rhs, Action<CSharpStatement> configureRhs = null)
    {
        rhs.Parent = this;
        Statements.Add(new CSharpObjectInitStatement(lhs, rhs));
        configureRhs?.Invoke(rhs);
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}{RelativeIndentation}{Text}
{base.GetText(indentation)}";
    }
}

public class CSharpObjectInitStatement : CSharpStatement, IHasCSharpStatements
{
    private readonly string _lhs;
    private readonly CSharpStatement _rhs;

    public CSharpObjectInitStatement(string lhs, CSharpStatement rhs) : base(null)
    {
        _lhs = lhs;
        _rhs = rhs;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; }

    public override string GetText(string indentation)
    {
        return $@"{indentation}{RelativeIndentation}{_lhs} = {_rhs.GetText(indentation).TrimStart()},";
    }
}