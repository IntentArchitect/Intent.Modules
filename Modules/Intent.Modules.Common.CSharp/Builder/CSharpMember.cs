namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpMember<TImpl> : CSharpDeclaration<TImpl>, ICodeBlock
    where TImpl : CSharpDeclaration<TImpl>
{
    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;

    public abstract string GetText(string indentation);
}