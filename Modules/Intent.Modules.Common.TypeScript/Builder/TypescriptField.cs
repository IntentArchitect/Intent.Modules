using System;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptField : TypescriptMember<TypescriptField>
{
    private bool _canBeNull;
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }

    public TypescriptField(string type, string name)
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

    public TypescriptField PrivateReadOnly()
    {
        AccessModifier = "private readonly ";
        return this;
    }

    public TypescriptField Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public TypescriptField CanBeNull()
    {
        _canBeNull = true;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}{Type}{(_canBeNull ? "?" : "")} {Name};";
    }
}