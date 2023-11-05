namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileScalarValue : IDataFileValue<IDataFileScalarValue>
{
    object Value { get; set; }
}