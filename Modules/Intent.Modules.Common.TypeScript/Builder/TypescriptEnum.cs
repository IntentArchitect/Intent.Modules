using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptEnum : TypescriptDeclaration<TypescriptEnum>, ICodeBlock
{
    public TypescriptEnum(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
    protected List<TypescriptEnumLiteral> Literals { get; private set; } = new List<TypescriptEnumLiteral>();
    public TypescriptCodeSeparatorType BeforeSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;
    public TypescriptCodeSeparatorType AfterSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;

    public bool IsExported { get; private set; }

    public TypescriptEnum AddLiteral(string literalName, string literalValue = null, Action<TypescriptEnumLiteral> configure = null)
    {
        var literal = new TypescriptEnumLiteral(literalName, literalValue);
        configure?.Invoke(literal);
        Literals.Add(literal);
        return this;
    }

    public TypescriptEnum Export()
    {
        IsExported = true;
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
        sb.Append(GetDecorators(indentation));
        sb.Append(indentation);

        sb.Append($"{(IsExported ? "export " : string.Empty)}enum ");
        sb.Append(Name);
        sb.Append(" {");
        sb.Append(indentation);
        sb.Append(Literals.JoinCode(",", $"{indentation}    "));
        sb.AppendLine();
        sb.Append(indentation);
        sb.Append('}');

        return sb.ToString();
    }
}
