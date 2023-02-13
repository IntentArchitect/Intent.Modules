using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitializerBlock : CSharpStatementBlock
{
    public CSharpObjectInitializerBlock(string invocation) : base(invocation)
    {
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
        return $@"{base.GetText(indentation)}";
    }
}