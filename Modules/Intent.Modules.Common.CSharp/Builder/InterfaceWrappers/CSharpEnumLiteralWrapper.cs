namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpEnumLiteralWrapper(CSharpEnumLiteral wrapped) :
    CSharpDeclarationWrapper<CSharpEnumLiteral, ICSharpEnumLiteral>(wrapped), ICSharpEnumLiteral
{
    public CSharpCodeSeparatorType BeforeSeparator
    {
        get => ((ICodeBlock)wrapped).BeforeSeparator;
        set => ((ICodeBlock)wrapped).BeforeSeparator = value;
    }

    public CSharpCodeSeparatorType AfterSeparator {
        get => ((ICodeBlock)wrapped).AfterSeparator;
        set => ((ICodeBlock)wrapped).AfterSeparator = value;
    }
    
    public string GetText(string indentation)
    {
        return ((ICodeBlock)wrapped).GetText(indentation);
    }

    public string LiteralName => wrapped.LiteralName;

    public string LiteralValue => wrapped.LiteralValue;
}