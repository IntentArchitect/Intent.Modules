using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpEnumLiteral : CSharpDeclaration<CSharpEnumLiteral>, ICSharpEnumLiteral
{
    private readonly CSharpEnumLiteralWrapper _wrapper;

    public CSharpEnumLiteral(string literalName, string literalValue)
    {
        if (string.IsNullOrWhiteSpace(literalName))
        {
            throw new ArgumentNullException(nameof(literalName));
        }
        
        _wrapper = new CSharpEnumLiteralWrapper(this);
        
        LiteralName = literalName;
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

    IEnumerable<ICSharpAttribute> ICSharpDeclaration<ICSharpEnumLiteral>.Attributes => _wrapper.Attributes;

    ICSharpEnumLiteral ICSharpDeclaration<ICSharpEnumLiteral>.AddAttribute(string name, Action<ICSharpAttribute> configure)
    {
        return _wrapper.AddAttribute(name, configure);
    }

    ICSharpEnumLiteral ICSharpDeclaration<ICSharpEnumLiteral>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure)
    {
        return _wrapper.AddAttribute(attribute, configure);
    }

    ICSharpEnumLiteral ICSharpDeclaration<ICSharpEnumLiteral>.WithComments(string xmlComments)
    {
        return _wrapper.WithComments(xmlComments);
    }

    ICSharpEnumLiteral ICSharpDeclaration<ICSharpEnumLiteral>.WithComments(IEnumerable<string> xmlComments)
    {
        return _wrapper.WithComments(xmlComments);
    }
}