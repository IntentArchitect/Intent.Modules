namespace Intent.Modules.Common.Java.Builder;

public class JavaGenericParameter
{
    public JavaGenericParameter(string typeName)
    {
        TypeName = typeName;
    }
    
    public string TypeName { get; }
    
    public override string ToString()
    {
        return TypeName;
    }

    public static implicit operator string(JavaGenericParameter param)
    {
        return param.ToString();
    }
}