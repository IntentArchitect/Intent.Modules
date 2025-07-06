#nullable enable
using System;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

internal class RazorDirective : IRazorDirective
{
    public string Keyword { get; }
    public ICSharpExpression? Expression { get; }

    public int Order { get; set; }

    public RazorDirective(string keyword, ICSharpExpression? expression = null)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(keyword));
        }

        Keyword = keyword;
        Expression = expression;
        Order = keyword switch
        {
            "page" => 0,
            "using" => 1,
            "attribute" => 2,
            "inject" => 3,
            _ => 0
        };
    }

    public override string ToString()
    {
        return $"@{Keyword}{(Expression != null ? $" {Expression}" : "")}";
    }
}