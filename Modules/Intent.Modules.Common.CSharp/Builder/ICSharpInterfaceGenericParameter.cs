namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpInterfaceGenericParameter : ICSharpGenericParameter
{
    GenericInterfaceType GenericInterfaceType { get; }
    ICSharpGenericParameter Covariant();
    ICSharpGenericParameter Contravariant();
    string GetText();
}