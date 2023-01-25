namespace Intent.Modules.Common.Java.Builder;

public interface ICodeBlock
{
    JavaCodeSeparatorType BeforeSeparator { get; set; }
    JavaCodeSeparatorType AfterSeparator { get; set; }
    string GetText(string indentation);
}