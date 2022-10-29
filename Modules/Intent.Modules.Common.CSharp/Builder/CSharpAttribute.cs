using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAttribute
{
    public string Name { get; set; }
    public IList<string> Arguments { get; set; } = new List<string>();
    public CSharpAttribute(string name)
    {
        if (name.StartsWith("[") && name.EndsWith("]"))
        {
            Name = name.Substring(1, name.Length - 2);
        }
        else
        {
            Name = name;
        }
    }

    public CSharpAttribute AddArgument(string name)
    {
        Arguments.Add(name);
        return this;
    }

    public override string ToString()
    {
        return $"[{Name}{(Arguments.Any() ? $"({string.Join(", ", Arguments)})" : string.Empty)}]";
    }
}