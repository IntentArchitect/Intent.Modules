using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptDecorator : TypescriptMetadataBase<TypescriptDecorator>, IHasTypescriptStatements
{
    public string Name { get; set; }
    public List<TypescriptStatement> Statements { get; set; } = new();
    public TypescriptDecorator(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name == "@")
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name.Length > 1 && name.StartsWith("@")
            ? name[1..]
            : name;
    }

    public TypescriptDecorator AddArgument(string name)
    {
        Statements.Add(new TypescriptStatement(name)
        {
            Parent = this
        });

        return this;
    }

    public virtual TypescriptDecorator FindAndReplace(string find, string replaceWith)
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
        return $"{indentation}@{Name}{(Statements.Any() ? $"({string.Join(", ", Statements.Select(x => x.ToString()))})" : string.Empty)}";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }
}