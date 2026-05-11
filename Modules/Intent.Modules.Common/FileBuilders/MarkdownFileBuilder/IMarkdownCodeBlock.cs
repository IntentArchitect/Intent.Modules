namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownCodeBlock : IMarkdownBlock
{
    /// <summary>Optional bold label rendered immediately before the code fence.</summary>
    string Title { get; set; }
    string Language { get; set; }
    string Code { get; set; }
}
