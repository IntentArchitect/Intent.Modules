using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAttribute : CSharpMetadataBase<CSharpAttribute>, IHasCSharpStatements
{
    public string Name { get; set; }
    public IList<CSharpStatement> Statements { get; set; } = new List<CSharpStatement>();
    public CSharpAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }
        
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
        var statement = new CSharpStatement(name);
        statement.Parent = this;
        Statements.Add(statement);
        return this;
    }

    public virtual CSharpAttribute FindAndReplace(string find, string replaceWith)
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
        return $"{indentation}[{Name}{(Statements.Any() ? $"({string.Join(", ", Statements.Select(x => x.ToString()))})" : string.Empty)}]";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }
}