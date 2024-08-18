#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// <see cref="ICSharpMethodDeclaration"/> can't be updated for backwards compatibility reasons, so
/// this was created for interfaces which needed to be "clean" of concretes.
/// </summary>
public interface ICSharpMethodDeclaration<out TCSharpMethodDeclaration> : ICSharpMethod<TCSharpMethodDeclaration>, ICSharpMember<TCSharpMethodDeclaration>, ICSharpReferenceable
    where TCSharpMethodDeclaration : ICSharpMethod<TCSharpMethodDeclaration>, ICSharpDeclaration<TCSharpMethodDeclaration>
{
    bool IsAbstract { get; }
    string? ExplicitImplementationFor { get; }
    TCSharpMethodDeclaration IsExplicitImplementationFor(string? @interface);
    TCSharpMethodDeclaration Protected();
    TCSharpMethodDeclaration Private();
    TCSharpMethodDeclaration WithoutAccessModifier();
    TCSharpMethodDeclaration Override();
    TCSharpMethodDeclaration New();
    TCSharpMethodDeclaration Virtual();
    TCSharpMethodDeclaration Abstract();
}