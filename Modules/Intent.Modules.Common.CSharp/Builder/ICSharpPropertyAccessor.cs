#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpPropertyAccessor
{
    string Accessor { get; }
    string AccessModifier { get; }
    bool IsExpression { get; }
    ICSharpPropertyAccessor Internal();
    ICSharpPropertyAccessor Private();
    ICSharpPropertyAccessor Protected();
    ICSharpPropertyAccessor ProtectedInternal();
    ICSharpPropertyAccessor Public();
    ICSharpPropertyAccessor Init();
    ICSharpPropertyAccessor WithImplementation(params string[] statements);
    ICSharpPropertyAccessor WithBodyImplementation(params string[] statements);
    ICSharpPropertyAccessor WithExpressionImplementation(string implementation);
}