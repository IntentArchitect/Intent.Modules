namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpMemberWrapper<TImpl, TSpecialization>(CSharpMember<TImpl> wrapped) :
    CSharpDeclarationWrapper<TImpl, TSpecialization>(wrapped), ICSharpMember<TSpecialization>
    where TImpl : CSharpDeclaration<TImpl>
    where TSpecialization : class, ICSharpDeclaration<TSpecialization>
{
    public CSharpCodeSeparatorType BeforeSeparator
    {
        get => wrapped.BeforeSeparator;
        set => wrapped.BeforeSeparator = value;
    }

    public CSharpCodeSeparatorType AfterSeparator
    {
        get => wrapped.AfterSeparator;
        set => wrapped.AfterSeparator = value;
    }

    public string GetText(string indentation)
    {
        return wrapped.GetText(indentation);
    }
}