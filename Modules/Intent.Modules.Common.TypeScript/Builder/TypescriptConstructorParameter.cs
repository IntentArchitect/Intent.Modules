using System;
using System.Collections.Generic;
using System.Linq;
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
    public IList<TypescriptDecorator> Decorators { get; } = new List<TypescriptDecorator>();

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

    public TypescriptConstructorParameter AddDecorator(string name, Action<TypescriptDecorator> configure = null)
    {
        var param = new TypescriptDecorator(name);
        Decorators.Add(param);
        configure?.Invoke(param);
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

        sb.Append(GetDecorators());

        if (AssignsToField && !string.IsNullOrWhiteSpace(AccessModifier))
        {
            sb.Append($"{AccessModifier} ");
        }

        if (AssignsToField && IsReadonly)
        {
            sb.Append("readonly ");
        }

        sb.Append(Name);

        sb.Append($": {Type}");

        if (IsOptional)
        {
            sb.Append(" | null");
        }

        return sb.ToString();
    }

    protected string GetDecorators()
    {
        return $"{(Decorators.Any() ? $"{string.Join(" ", Decorators)} " : string.Empty)}";
    }
}