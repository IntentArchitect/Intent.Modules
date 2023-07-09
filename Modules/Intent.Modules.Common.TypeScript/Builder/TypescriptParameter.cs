using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptParameter
{
    public string Type { get; }
    public string Name { get; }
    public string DefaultValue { get; private set; }
    public IList<TypescriptDecorator> Decorators { get; } = new List<TypescriptDecorator>();

    public TypescriptParameter(string name, string type)
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

    public TypescriptParameter AddDecorator(string name, Action<TypescriptDecorator> configure = null)
    {
        var param = new TypescriptDecorator(name);
        Decorators.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public TypescriptParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public override string ToString()
    {
        return $"{GetDecorators()}{Name}: {Type}{(DefaultValue != null ? $" = {DefaultValue}" : string.Empty)}";
    }

    protected string GetDecorators()
    {
        return $"{(Decorators.Any() ? $"{string.Join(" ", Decorators)} " : string.Empty)}";
    }
}