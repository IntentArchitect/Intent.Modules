using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAttribute : CSharpMetadataBase<CSharpAttribute>, ICSharpAttribute, IHasCSharpStatements
{
    public string Name { get; set; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public CSharpAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name == "[]")
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name.StartsWith("[") && name.EndsWith("]")
            ? name[1..^1]
            : name;
    }


    public CSharpAttribute AddArgument(string name)
    {
        Statements.Add(new CSharpStatement(name)
        {
            Parent = this
        });

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

    bool IHasCSharpStatementsActual.IsCodeBlock => false;

    #region ICSharpAttribute

    ICSharpAttribute ICSharpAttribute.FindAndReplace(string find, string replaceWith) =>
        FindAndReplace(find, replaceWith);

    ICSharpAttribute ICSharpAttribute.AddArgument(string name) =>
        AddArgument(name);

    #endregion
}