namespace Intent.Modules.Common.TypeScript.Builder;

public abstract class TypescriptMember<TImpl> : TypescriptDeclaration<TImpl>, ICodeBlock
    where TImpl : TypescriptDeclaration<TImpl>
{
    public TypescriptCodeSeparatorType BeforeSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;
    public TypescriptCodeSeparatorType AfterSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;

    public abstract string GetText(string indentation);
}