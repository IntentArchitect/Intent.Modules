#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpEnum : CSharpDeclaration<CSharpEnum>, ICodeBlock
{
    public CSharpEnum(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
    internal string AccessModifier { get; private set; } = "public ";
    protected List<CSharpEnumLiteral> Literals { get; private set; } = new List<CSharpEnumLiteral>();
    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    
    public CSharpEnum Internal()
    {
        AccessModifier = "internal ";
        return this;
    }

    /// <summary>
    /// Obsolete. Use <see cref="ProtectedInternal"/> instead.
    /// </summary>
    public CSharpEnum InternalProtected()
    {
        AccessModifier = "internal protected ";
        return this;
    }

    public CSharpEnum Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpEnum Protected()
    {
        AccessModifier = "protected ";
        return this;
    }

    public CSharpEnum ProtectedInternal()
    {
        AccessModifier = "protected internal ";
        return this;
    }

    public CSharpEnum Public()
    {
        AccessModifier = "public ";
        return this;
    }

    public CSharpEnum AddLiteral(string literalName, string literalValue = null, Action<CSharpEnumLiteral> configure = null)
    {
        var literal = new CSharpEnumLiteral(literalName, literalValue);
        configure?.Invoke(literal);
        Literals.Add(literal);
        return this;
    }
    
    public string GetText(string indentation)
    {
        return ToString(indentation);
    }
    
    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        var sb = new StringBuilder();
        
        sb.Append(GetComments(indentation));
        sb.Append(GetAttributes(indentation));
        sb.Append(indentation);
        sb.Append(AccessModifier);

        sb.Append("enum ");
        sb.Append(Name);
        sb.AppendLine();
        
        sb.Append(indentation);
        sb.Append("{");
        sb.Append(Literals.JoinCode(",", $"{indentation}    "));
        sb.AppendLine();
        sb.Append(indentation);
        sb.Append("}");

        return sb.ToString();
    }
}