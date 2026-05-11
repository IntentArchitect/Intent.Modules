namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public class MarkdownCodeBlock : IMarkdownCodeBlock
{
    public MarkdownCodeBlock(string code, string language = null, string title = null)
    {
        Code = code;
        Language = language;
        Title = title;
    }

    public string Title { get; set; }
    public string Language { get; set; }
    public string Code { get; set; }
}
