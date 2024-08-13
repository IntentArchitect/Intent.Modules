namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// <see cref="ICSharpMethodDeclaration"/> can't be updated for backwards compatibility reasons, so
/// this was created for interfaces which needed to be "clean" of concretes.
/// </summary>
public interface ICSharpMethodDeclarationActual : IHasICSharpParameters, ICSharpReferenceable, IHasCSharpStatementsActual
{
    bool IsAsync { get; }
    ICSharpExpression ReturnType { get; }
}

public interface ICSharpMethodDeclaration : IHasICSharpParameters, ICSharpReferenceable, IHasCSharpStatements
{
    bool IsAsync { get; }
    public ICSharpExpression ReturnType { get; }
    public CSharpType ReturnTypeInfo { get; }
}