namespace Intent.Modules.Common.CSharp.Builder;

public interface ICodeBlock
{
    public CSharpCodeSeparatorType BeforeSeparator { get; set; }
    public CSharpCodeSeparatorType AfterSeparator { get; set; }
    public string GetText(string indentation);
}