namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileBuilderTemplate : IFileBuilderTemplate
{
    public new IDataFile DataFile { get; }

    IFileBuilderBase IFileBuilderTemplate.File => DataFile;
}