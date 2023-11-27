namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public interface IIndentedFileContent : IIndentedFileItem<IIndentedFileContent>
{
    string Content { get; set; }
}