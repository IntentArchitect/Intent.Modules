namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownListItem : IMarkdownBlock
{
    string Content { get; set; }
    bool IsOrdered { get; }

    /// <summary>
    /// Zero-based nesting depth. 0 = top-level, 1 = one indent (2 spaces), etc.
    /// </summary>
    int IndentLevel { get; }
}
