#nullable enable
using System;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

internal class RazorCodeDirective : RazorFileNodeBase<RazorCodeDirective, IRazorCodeDirective>, IRazorCodeDirective
{
    public ICSharpExpression Expression { get; set; }
    public RazorCodeDirective(ICSharpExpression expression, IRazorFile file) : base(file)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public override string GetText(string indentation)
    {
        if (ChildNodes.Count == 0)
        {
            return $"{indentation}{Expression.GetText(indentation)?.TrimStart()}";
        }
        return $@"{indentation}{Expression.GetText(indentation)?.TrimStart()} 
{indentation}{{
{string.Join("", ChildNodes.Select(x => x.GetText($"{indentation}    "))).TrimEnd()}
{indentation}}}
";
    }
}