using System;
using System.Linq;
using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Builder;

public class JavaAnnotation : JavaMetadataBase<JavaAnnotation>, IHasJavaStatements
{
    public string Name { get; set; }
    public IList<JavaStatement> Statements { get; } = new List<JavaStatement>();
    public JavaAnnotation(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name.StartsWith("@") ? name[1..] : name;
    }

    public JavaAnnotation AddArgument(string name)
    {
        Statements.Add(new JavaStatement(name)
        {
            Parent = this
        });

        return this;
    }

    public virtual JavaAnnotation FindAndReplace(string find, string replaceWith)
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