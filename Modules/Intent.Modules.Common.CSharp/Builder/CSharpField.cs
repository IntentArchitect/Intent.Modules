namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpField : CSharpDeclaration<CSharpField>
{
    private bool _canBeNull = false;
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string AccessModifier { get; private set; }

    public CSharpField(string type, string name)
    {
        AccessModifier = "private ";
        Type = type;
        Name = name;
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

    public CSharpField CanBeNull()
    {
        _canBeNull = true;
        return this;
    }

    public string ToString(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{Type}{(_canBeNull ? "?" : "")} {Name};";
    }
}