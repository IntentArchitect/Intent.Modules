#nullable enable
#pragma warning disable CS1591
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpClassMethodDeclaration : ICSharpMethodDeclaration<ICSharpClassMethodDeclaration>
{
    ICSharpClass Class { get; }
}