using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAssemblyAttribute : CSharpMetadataBase<CSharpAssemblyAttribute>, IHasCSharpStatements
{
    public string Name { get; set; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public CSharpAssemblyAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name == "[]")
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        name = name.Trim();

        if (name.StartsWith("[") && name.EndsWith("]"))
        {
            name = name[1..^1].Trim();
        }

        if (name.StartsWith("assembly:"))
        {
            name = name["assembly:".Length..].Trim();
        }

        Name = name;
    }

    public CSharpAssemblyAttribute AddArgument(string name)
    {
        Statements.Add(new(name)
        {
            Parent = this
        });

        return this;
    }

    public virtual CSharpAssemblyAttribute FindAndReplace(string find, string replaceWith)
    {
        Name = Name.Replace(find, replaceWith);

        foreach (var argument in Statements)
        {
            argument.FindAndReplace(find, replaceWith);
        }

        return this;
    }

    public virtual string GetText(string indentation)
    {
        return $"{indentation}[assembly: {Name}{(Statements.Any() ? $"({string.Join(", ", Statements.Select(x => x.ToString()))})" : string.Empty)}]";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }
}