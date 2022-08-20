namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpField
{
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }

    public CSharpField(string type, string name)
    {
        AccessModifier = "private ";
        Type = type;
        Name = name;
    }
    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier}{Type} {Name};";
    }

    public CSharpField PrivateReadOnly()
    {
        AccessModifier = "private readonly ";
        return this;
    }
    public CSharpField Private()
    {
        AccessModifier = "private ";
        return this;
    }
}