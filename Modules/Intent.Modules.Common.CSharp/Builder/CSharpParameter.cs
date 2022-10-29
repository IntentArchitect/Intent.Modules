using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpParameter
{
    public string Type { get; }
    public string Name { get; }
    public string DefaultValue { get; private set; }
    public IList<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();

    public CSharpParameter(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public CSharpParameter AddAttribute(string name, Action<CSharpAttribute> configure = null)
    {
        var param = new CSharpAttribute(name);
        Attributes.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public override string ToString()
    {
        return $@"{GetAttributes()}{Type} {Name}{(DefaultValue != null ? $" = {DefaultValue}" : string.Empty)}";
    }

    protected string GetAttributes()
    {
        return $@"{(Attributes.Any() ? $@"{string.Join($@" ", Attributes)} " : string.Empty)}";
    }
}