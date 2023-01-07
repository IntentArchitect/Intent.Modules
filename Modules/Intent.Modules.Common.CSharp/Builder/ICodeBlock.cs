namespace Intent.Modules.Common.CSharp.Builder;

public interface ICodeBlock
{
    CSharpCodeSeparatorType BeforeSeparator { get; set; }
    CSharpCodeSeparatorType AfterSeparator { get; set; }
    string GetText(string indentation);
}