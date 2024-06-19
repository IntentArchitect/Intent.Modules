namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceGenericParameter : CSharpGenericParameter, ICSharpInterfaceGenericParameter
{
    public CSharpInterfaceGenericParameter(string typeName) 
        : base(typeName)
    {
        GenericInterfaceType = GenericInterfaceType.Default;
    }
    
    public GenericInterfaceType GenericInterfaceType { get; private set; }

    ICSharpGenericParameter ICSharpInterfaceGenericParameter.Covariant() => Covariant();
    public CSharpGenericParameter Covariant()
    {
        GenericInterfaceType = GenericInterfaceType.Covariant;
        return this;
    }

    ICSharpGenericParameter ICSharpInterfaceGenericParameter.Contravariant() => Contravariant();
    public CSharpGenericParameter Contravariant()
    {
        GenericInterfaceType = GenericInterfaceType.Contravariant;
        return this;
    }
    
    public string GetText()
    {
        return $"{GetVariantText()}{TypeName}";
    }

    private string GetVariantText()
    {
        return GenericInterfaceType switch
        {
            GenericInterfaceType.Default => string.Empty,
            GenericInterfaceType.Covariant => "out ",
            GenericInterfaceType.Contravariant => "in "
        };
    }
}