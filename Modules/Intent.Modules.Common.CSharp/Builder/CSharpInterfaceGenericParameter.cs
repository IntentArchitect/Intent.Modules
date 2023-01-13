namespace Intent.Modules.Common.CSharp.Builder;

public enum GenericInterfaceType
{
    Default = 0,
    Covariant = 1,
    Contravariant = 2
}

public class CSharpInterfaceGenericParameter : CSharpGenericParameter
{
    public CSharpInterfaceGenericParameter(string typeName) 
        : base(typeName)
    {
        GenericInterfaceType = GenericInterfaceType.Default;
    }
    
    public GenericInterfaceType GenericInterfaceType { get; private set; }

    public CSharpGenericParameter Covariant()
    {
        GenericInterfaceType = GenericInterfaceType.Covariant;
        return this;
    }
    
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