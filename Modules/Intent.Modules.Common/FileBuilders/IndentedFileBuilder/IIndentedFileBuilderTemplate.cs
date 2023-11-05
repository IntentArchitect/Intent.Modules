namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public interface IIndentedFileBuilderTemplate : IFileBuilderTemplate
{
    public new IIndentedFile IndentedFile { get; }

    IFileBuilderBase IFileBuilderTemplate.File => IndentedFile;
}