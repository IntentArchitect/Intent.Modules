using System;
using System.Net.Http.Headers;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptField : TypescriptMember<TypescriptField>
{
    public string Type { get; }
    public string Name { get; }
    public string Value { get; private set; }
    public string AccessModifier { get; private set; } = string.Empty;
    public bool IsDefinitelyAssigned { get; private set; } = false;


    public TypescriptField(string name, string type)
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

    public TypescriptField(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

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

    public TypescriptField DefinitelyAssigned()
    {
        IsDefinitelyAssigned = true;
        return this;
    }

    public TypescriptField WithValue(string value)
    {
        Value = value;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}{Name}{(IsDefinitelyAssigned ? "!" : string.Empty)}{(!string.IsNullOrWhiteSpace(Type) ? $": {Type}" : string.Empty)}{(!string.IsNullOrWhiteSpace(Value) ? $" = {Value}" : string.Empty )};";
    }
}