namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownFileBuilderTemplate : IFileBuilderTemplate
{
    new IMarkdownFile MarkdownFile { get; }
    IFileBuilderBase IFileBuilderTemplate.File => MarkdownFile;
}
