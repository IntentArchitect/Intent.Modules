namespace Intent.Modules.Common.Java.Builder;

public abstract class JavaMember<TImpl> : JavaDeclaration<TImpl>, ICodeBlock
    where TImpl : JavaDeclaration<TImpl>
{
    public JavaCodeSeparatorType BeforeSeparator { get; set; } = JavaCodeSeparatorType.NewLine;
    public JavaCodeSeparatorType AfterSeparator { get; set; } = JavaCodeSeparatorType.NewLine;

    public abstract string GetText(string indentation);
}