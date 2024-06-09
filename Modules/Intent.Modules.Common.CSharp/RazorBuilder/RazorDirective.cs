using System;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public class RazorDirective
{
    public string Keyword { get; }
    public ICSharpExpression Expression { get; }

    public int Order { get; set; }

    public RazorDirective(string keyword, ICSharpExpression expression = null)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(keyword));
        }

        Keyword = keyword;
        Expression = expression;
        switch (keyword)
        {
            case "page":
                Order = 0;
                break;
            case "using":
                Order = 1;
                break;
            case "inject":
                Order = 2;
                break;
            default:
                Order = 0;
                break;
        }
    }

    public override string ToString()
    {
        return $"@{Keyword}{(Expression != null ? $" {Expression}" : "")}";
    }
}