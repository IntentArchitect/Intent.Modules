namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileValueVisitor
{
    void Visit(IDataFileValue element);
    void Visit(IDataFileDictionaryValue dictionary);
    void Visit(IDataFileArrayValue array);
    void Visit(IDataFileScalarValue scalar);
}