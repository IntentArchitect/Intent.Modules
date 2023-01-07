using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpField : CSharpMember<CSharpField>
{
    private bool _canBeNull;
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }

    public CSharpField(string type, string name)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

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

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{Type}{(_canBeNull ? "?" : "")} {Name};";
    }
}