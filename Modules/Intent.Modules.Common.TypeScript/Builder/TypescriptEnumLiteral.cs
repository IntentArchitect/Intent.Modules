using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptEnumLiteral : TypescriptDeclaration<TypescriptEnumLiteral>, ICodeBlock
{
    public TypescriptEnumLiteral(string literalName, string literalValue)
    {
        LiteralName = literalName ?? throw new ArgumentNullException(nameof(literalName));
        LiteralValue = literalValue;
    }

    public string LiteralName { get; }
    public string LiteralValue { get; }

    public TypescriptCodeSeparatorType BeforeSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;
    public TypescriptCodeSeparatorType AfterSeparator { get; set; } = TypescriptCodeSeparatorType.None;

    public string GetText(string indentation)
    {
        var sb = new StringBuilder();

        sb.Append(GetComments(indentation));
        sb.Append(GetDecorators(indentation));
        sb.Append(indentation);
        sb.Append(LiteralName);

        if (!string.IsNullOrWhiteSpace(LiteralValue))
        {
            sb.Append($" = {LiteralValue}");
        }

        return sb.ToString();
    }
}
