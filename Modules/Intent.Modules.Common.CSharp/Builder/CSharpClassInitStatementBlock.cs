using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpClassInitStatementBlock : CSharpStatementBlock
{
    public CSharpClassInitStatementBlock(string invocation) : base()
    {
        Text = invocation;
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public new CSharpClassInitStatementBlock WithSemicolon()
    {
        base.WithSemicolon();
        return this;
    }

    public CSharpClassInitStatementBlock AddInitAssignment(string lhs, CSharpStatement rhs)
    {
        Statements.Add(new CSharpClassInitAssignment(lhs, rhs));
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}{RelativeIndentation}{Text}
{base.GetText(indentation)}";
    }
}

public class CSharpClassInitAssignment : CSharpStatement, IHasCSharpStatements
{
    private readonly string _lhs;
    private readonly CSharpStatement _rhs;

    public CSharpClassInitAssignment(string lhs, CSharpStatement rhs) : base(null)
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