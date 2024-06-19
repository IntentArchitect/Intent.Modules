namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpGenericParameter : ICSharpGenericParameter
{
    public CSharpGenericParameter(string typeName)
    {
        TypeName = typeName;
    }
    
    public string TypeName { get; }
    
    public override string ToString()
    {
        return TypeName;
    }

    public static implicit operator string(CSharpGenericParameter param)
    {
        return param.ToString();
    }
}