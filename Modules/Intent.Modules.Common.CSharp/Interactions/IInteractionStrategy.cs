#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Interactions;

public interface IInteractionStrategy
{
    bool IsMatch(IElement interaction);
    void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interaction);
}