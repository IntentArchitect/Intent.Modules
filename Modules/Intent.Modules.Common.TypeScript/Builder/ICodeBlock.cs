namespace Intent.Modules.Common.TypeScript.Builder;

public interface ICodeBlock
{
    TypescriptCodeSeparatorType BeforeSeparator { get; set; }
    TypescriptCodeSeparatorType AfterSeparator { get; set; }
    string GetText(string indentation);
}