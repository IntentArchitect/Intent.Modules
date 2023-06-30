namespace Intent.Modules.Common.Java.Builder;

public class JavaInterfaceGenericParameter : JavaGenericParameter
{
    public JavaInterfaceGenericParameter(string typeName) : base(typeName)
    {
    }

    // Keeping this in the event where we need to add constraints
    public string GetText()
    {
        return ToString();
    }
}