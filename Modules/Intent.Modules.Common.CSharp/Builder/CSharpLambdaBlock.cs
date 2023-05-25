using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpLambdaBlock : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon;

    public CSharpLambdaBlock(string invocation) : base(invocation)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public bool HasExpressionBody { get; private set; }

    public CSharpLambdaBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }
    
    public CSharpLambdaBlock WithExpressionBody(CSharpStatement statement)
    {
        HasExpressionBody = true;
        statement.BeforeSeparator = CSharpCodeSeparatorType.None;
        if (statement is CSharpMethodChainStatement stmt)
        {
            stmt.WithoutSemicolon();
        }
        Statements.Add(statement);
        return this;
    }

    public override string GetText(string indentation)
    {
        if (HasExpressionBody)
        {
            var expressionBody = Statements.ConcatCode($"{indentation}{RelativeIndentation}");
            return $@"{base.GetText(indentation)} => {expressionBody}{(_withSemicolon ? ";" : string.Empty)}";
        }
        
        return @$"{base.GetText(indentation)} =>
{indentation}{RelativeIndentation}{{{Statements.ConcatCode($"{indentation}{RelativeIndentation}    ")}
{indentation}{RelativeIndentation}}}{(_withSemicolon ? ";" : string.Empty)}";
    }
}