using System;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpEnumLiteral : CSharpDeclaration<CSharpEnumLiteral>, ICodeBlock
{
    public CSharpEnumLiteral(string literalName, string literalValue)
    {
        LiteralName = literalName ?? throw new ArgumentNullException(nameof(literalName));
        LiteralValue = literalValue;
    }
    
    public string LiteralName { get; }
    public string LiteralValue { get; }

    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.EmptyLines;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    
    public string GetText(string indentation)
    {
        var sb = new StringBuilder();
        
        sb.Append(GetComments(indentation));
        sb.Append(GetAttributes(indentation));
        sb.Append(indentation);
        sb.Append(LiteralName);

        if (!string.IsNullOrWhiteSpace(LiteralValue))
        {
            sb.Append($" = {LiteralValue}");
        }

        return sb.ToString();
    }
}