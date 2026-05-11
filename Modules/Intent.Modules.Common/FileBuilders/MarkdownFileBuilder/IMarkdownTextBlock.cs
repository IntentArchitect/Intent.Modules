namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownTextBlock : IMarkdownBlock
{
    string Text { get; set; }
}
