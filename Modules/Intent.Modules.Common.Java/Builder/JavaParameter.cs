using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaParameter
{
    public string Type { get; }
    public string Name { get; }
    public string DefaultValue { get; private set; }
    public bool IsFinal { get; private set; }
    public IList<JavaAnnotation> Annotations { get; } = new List<JavaAnnotation>();

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

    public JavaParameter AddAnnotation(string name, Action<JavaAnnotation> configure = null)
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

    public JavaParameter Final()
    {
        IsFinal = true;
        return this;
    }

    public override string ToString()
    {
        return $@"{(IsFinal ? "final " : string.Empty)}{GetAttributes()}{Type} {Name}{(DefaultValue != null ? $" = {DefaultValue}" : string.Empty)}";
    }

    protected string GetAttributes()
    {
        return $@"{(Annotations.Any() ? $@"{string.Join(@" ", Annotations)} " : string.Empty)}";
    }
}