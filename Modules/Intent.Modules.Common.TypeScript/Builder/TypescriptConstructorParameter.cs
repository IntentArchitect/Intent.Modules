using System;
using System.Text;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptConstructorParameter
{
    public string Type { get; }
    public string Name { get; }
    public bool AssignsToField { get; private set; }
    public bool IsReadonly { get; private set; }
    public bool IsOptional { get; private set; }
    public string AccessModifier { get; private set; } = string.Empty;

    public TypescriptConstructorParameter(string name, string type)
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

    public TypescriptConstructorParameter Optional()
    {
        IsOptional = true;

        return this;
    }

    public TypescriptConstructorParameter WithFieldAssignment(string accessModifier = "public", bool isReadonly = false)
    {
        if (accessModifier is not ("public" or "protected" or "private"))
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(accessModifier),
                actualValue: accessModifier,
                message: "Must be \"public\", \"protected\" or \"private\"");
        }

        AssignsToField = true;
        IsReadonly = isReadonly;
        AccessModifier = accessModifier;

        return this;
    }

    public TypescriptConstructorParameter WithPrivateFieldAssignment()
    {
        return WithFieldAssignment("private");
    }

    public TypescriptConstructorParameter WithPrivateReadonlyFieldAssignment()
    {
        return WithFieldAssignment("private", true);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (AssignsToField && !string.IsNullOrWhiteSpace(AccessModifier))
        {
            sb.Append($"{AccessModifier} ");
        }

        if (AssignsToField && IsReadonly)
        {
            sb.Append("readonly ");
        }

        sb.Append(Name);

        if (IsOptional)
        {
            sb.Append("?");
        }

        sb.Append($": {Type}");

        return sb.ToString();
    }
}