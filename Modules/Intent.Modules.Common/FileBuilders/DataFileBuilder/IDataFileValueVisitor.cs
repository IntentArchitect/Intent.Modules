namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileValueVisitor
{
    void Visit(IDataFileValue element);
    void Visit(IDataFileObjectValue @object);
    void Visit(IDataFileArrayValue array);
    void Visit(IDataFileScalarValue scalar);
}