#nullable enable
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorCodeDirective : IRazorFileNodeBase<IRazorCodeDirective>
{
    static IRazorCodeDirective Create(ICSharpExpression expression, IRazorFile file)
    {
        return new RazorCodeDirective(expression, file);
    }

    ICSharpExpression Expression { get; set; }
}