using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitializerBlock : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon;
    
    public CSharpObjectInitializerBlock(string invocation) : base(invocation)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }
    
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public new CSharpObjectInitializerBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public CSharpObjectInitializerBlock AddInitStatement(string lhs, CSharpStatement rhs, Action<CSharpStatement> configureRhs = null)
    {
        rhs.Parent = this;
        Statements.Add(new CSharpObjectInitStatement(lhs, rhs));
        configureRhs?.Invoke(rhs);
        return this;
    }

    public CSharpObjectInitializerBlock AddKeyAndValue(CSharpStatement key, CSharpStatement value)
    {
        Statements.Add(new CSharpObjectInitKeyValueStatement(key, value));
        return this;
    }

    public override string GetText(string indentation)
    {
        return @$"{(Text.Length > 0 ? base.GetText(indentation) + Environment.NewLine : "")}{indentation}{RelativeIndentation}{{{Statements.JoinCode(",", $"{indentation}{RelativeIndentation}    ")}
{indentation}{RelativeIndentation}}}{(_withSemicolon ? ";" : "")}";
    }
}