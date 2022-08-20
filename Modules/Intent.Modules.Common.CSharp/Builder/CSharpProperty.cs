namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpProperty
{
    public string AccessModifier { get; private set; } = "public ";
    public string Type { get; }
    public string Name { get; }
    public string Getter { get; private set; } = "get;";
    public string Setter { get; private set; } = " set;";

    public CSharpProperty(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public CSharpProperty Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpProperty Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpProperty PrivateSetter()
    {
        Setter = " private set;";
        return this;
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier}{Type} {Name} {{ {Getter}{Setter} }}";
    }
}