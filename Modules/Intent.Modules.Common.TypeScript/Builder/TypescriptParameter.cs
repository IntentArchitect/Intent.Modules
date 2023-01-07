using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptParameter
{
    public string Type { get; }
    public string Name { get; }
    public string DefaultValue { get; private set; }
    public IList<TypescriptAttribute> Attributes { get; } = new List<TypescriptAttribute>();
    public bool HasThisModifier { get; private set; }

    public TypescriptParameter(string type, string name)
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

    public TypescriptParameter AddAttribute(string name, Action<TypescriptAttribute> configure = null)
    {
        var param = new TypescriptAttribute(name);
        Attributes.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public TypescriptParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public TypescriptParameter WithThisModifier()
    {
        HasThisModifier = true;
        return this;
    }

    public override string ToString()
    {
        return $@"{(HasThisModifier ? "this " : string.Empty)}{GetAttributes()}{Type} {Name}{(DefaultValue != null ? $" = {DefaultValue}" : string.Empty)}";
    }

    protected string GetAttributes()
    {
        return $@"{(Attributes.Any() ? $@"{string.Join(@" ", Attributes)} " : string.Empty)}";
    }
}