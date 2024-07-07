#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorDirective
{
    ICSharpExpression? Expression { get; }

    string Keyword { get; }

    int Order { get; set; }
}