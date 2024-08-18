namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpMethodDeclaration : IHasICSharpParameters, ICSharpReferenceable, IHasCSharpStatements
{
    bool IsAsync { get; }
    public ICSharpExpression ReturnType { get; }
    public CSharpType ReturnTypeInfo { get; }
}