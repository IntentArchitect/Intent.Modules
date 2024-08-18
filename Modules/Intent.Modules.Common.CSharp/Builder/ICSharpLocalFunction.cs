#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpLocalFunction : ICSharpMethod<ICSharpLocalFunction>
{
    new IHasCSharpStatements Parent { get; }
}