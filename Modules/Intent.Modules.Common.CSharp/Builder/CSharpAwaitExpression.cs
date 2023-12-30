namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAwaitExpression : CSharpStatement
{
    public ICSharpExpression Expression { get; }

    public CSharpAwaitExpression(ICSharpExpression expression)
    {
        Expression = expression;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{indentation}await {Expression.GetText(indentation).TrimStart()}";
    }
}