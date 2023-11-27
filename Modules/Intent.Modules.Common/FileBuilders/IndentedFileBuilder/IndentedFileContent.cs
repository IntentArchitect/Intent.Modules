namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public class IndentedFileContent : IndentedFileItem<IIndentedFileContent>, IIndentedFileContent
{
    public IndentedFileContent(string content)
    {
        Content = content;
    }

    public string Content { get; set; }
}