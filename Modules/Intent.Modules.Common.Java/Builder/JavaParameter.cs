using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaParameter
{
    public string Type { get; }
    public string Name { get; }
    public string DefaultValue { get; private set; }
    public IList<JavaAnnotation> Annotations { get; } = new List<JavaAnnotation>();
    public bool HasThisModifier { get; private set; }

    public JavaParameter(string type, string name)
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

    public JavaParameter AddAttribute(string name, Action<JavaAnnotation> configure = null)
    {
        var param = new JavaAnnotation(name);
        Annotations.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public JavaParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public JavaParameter WithThisModifier()
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
        return $@"{(Annotations.Any() ? $@"{string.Join(@" ", Annotations)} " : string.Empty)}";
    }
}