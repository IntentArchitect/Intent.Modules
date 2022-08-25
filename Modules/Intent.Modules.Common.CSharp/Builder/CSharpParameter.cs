namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpParameter
{
    public string Type { get; }
    public string Name { get; }
    public string DefaultValue { get; private set; }

    public CSharpParameter(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public CSharpParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public override string ToString()
    {
        return $@"{Type} {Name}{(DefaultValue != null ? $" = {DefaultValue}" : string.Empty)}";
    }
}