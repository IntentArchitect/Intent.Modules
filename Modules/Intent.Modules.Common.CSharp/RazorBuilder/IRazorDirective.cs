#nullable enable
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorDirective
{
    ICSharpExpression? Expression { get; }

    string Keyword { get; }

    int Order { get; set; }
}