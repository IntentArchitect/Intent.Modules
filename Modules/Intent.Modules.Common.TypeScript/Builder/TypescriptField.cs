using System;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptField : TypescriptMember<TypescriptField>
{
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; } = string.Empty;

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

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier} {Name}: {Type};";
    }
}