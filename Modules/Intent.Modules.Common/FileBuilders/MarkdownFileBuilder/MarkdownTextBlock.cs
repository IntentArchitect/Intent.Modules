namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public class MarkdownTextBlock : IMarkdownTextBlock
{
    public MarkdownTextBlock(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
}
