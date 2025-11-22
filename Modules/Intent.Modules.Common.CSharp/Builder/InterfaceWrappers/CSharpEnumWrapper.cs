using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpEnumWrapper(CSharpEnum wrapped) :
    CSharpDeclarationWrapper<CSharpEnum, ICSharpEnum>(wrapped), ICSharpEnum
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

    string IHasCSharpName.Name => wrapped.Name;

    public IList<ICSharpEnumLiteral> Literals => new WrappedList<CSharpEnumLiteral, ICSharpEnumLiteral>(wrapped.Literals);
}