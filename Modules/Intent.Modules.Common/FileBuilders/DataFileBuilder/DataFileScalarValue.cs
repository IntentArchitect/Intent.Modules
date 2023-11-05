namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public class DataFileScalarValue : DataFileValue<IDataFileScalarValue>, IDataFileScalarValue
{
    public DataFileScalarValue(object value)
    {
        Value = value;
    }

    public object Value { get; set; }

    public static implicit operator DataFileScalarValue(bool value) => new(value);
    public static implicit operator DataFileScalarValue(byte value) => new(value);
    public static implicit operator DataFileScalarValue(sbyte value) => new(value);
    public static implicit operator DataFileScalarValue(char value) => new(value);
    public static implicit operator DataFileScalarValue(decimal value) => new(value);
    public static implicit operator DataFileScalarValue(double value) => new(value);
    public static implicit operator DataFileScalarValue(float value) => new(value);
    public static implicit operator DataFileScalarValue(int value) => new(value);
    public static implicit operator DataFileScalarValue(uint value) => new(value);
    public static implicit operator DataFileScalarValue(long value) => new(value);
    public static implicit operator DataFileScalarValue(ulong value) => new(value);
    public static implicit operator DataFileScalarValue(short value) => new(value);
    public static implicit operator DataFileScalarValue(ushort value) => new(value);
    public static implicit operator DataFileScalarValue(string value) => new(value);
}