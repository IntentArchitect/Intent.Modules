namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public class MarkdownListItem : IMarkdownListItem
{
    public MarkdownListItem(string content, bool isOrdered = false, int indentLevel = 0)
    {
        Content = content;
        IsOrdered = isOrdered;
        IndentLevel = indentLevel;
    }

    public string Content { get; set; }
    public bool IsOrdered { get; }
    public int IndentLevel { get; }
}
