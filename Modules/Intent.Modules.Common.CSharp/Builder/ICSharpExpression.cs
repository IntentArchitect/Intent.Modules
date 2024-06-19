namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpExpression
{
    ICSharpReferenceable Reference { get; }
    string GetText(string indentation);
    string ToString();
}