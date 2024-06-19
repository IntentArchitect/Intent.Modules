#nullable enable
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpPropertyAccessor
{
    string Accessor { get; }
    string AccessModifier { get; }
    bool IsExpression { get; }
    ICSharpPropertyAccessor Public();
    ICSharpPropertyAccessor Protected();
    ICSharpPropertyAccessor Internal();
    ICSharpPropertyAccessor Private();
    ICSharpPropertyAccessor Init();
    ICSharpPropertyAccessor WithImplementation(params string[] statements);
    ICSharpPropertyAccessor WithBodyImplementation(params string[] statements);
    ICSharpPropertyAccessor WithExpressionImplementation(string implementation);
}