namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpMember<TImpl> : CSharpDeclaration<TImpl>, ICodeBlock
    where TImpl : CSharpDeclaration<TImpl>
{
    public CSharpCodeSeparatorType Separator { get; set; } = CSharpCodeSeparatorType.None;

    public abstract string GetText(string indentation);
}