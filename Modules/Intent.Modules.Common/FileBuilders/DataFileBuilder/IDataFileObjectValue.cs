using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileObjectValue : IDataFileValue<IDataFileObjectValue>, IDictionary<string, DataFileValue>
{
    bool BlankLinesBetweenItems { get; set; }
    IDataFileObjectValue WithArray(string name, Action<IDataFileArrayValue> configure = default);
    IDataFileObjectValue WithArray(string name, int index, Action<IDataFileArrayValue> configure = default);
    IDataFileObjectValue WithObject(string name, Action<IDataFileObjectValue> configure = default);
    IDataFileObjectValue WithObject(string name, int index, Action<IDataFileObjectValue> configure = default);
    IDataFileObjectValue WithValue(string name, bool value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, bool value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, byte value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, byte value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, char value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, char value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, decimal value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, decimal value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, double value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, double value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, float value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, float value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, int value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, int value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, long value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, long value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, sbyte value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, sbyte value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, short value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, short value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, string value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, string value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, uint value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, uint value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, ulong value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, ulong value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, ushort value, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue(string name, ushort value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileObjectValue WithValue<TValue>(string name, TValue value, Action<TValue> configure = default) where TValue : IDataFileValue;
    IDataFileObjectValue WithValue<TValue>(string name, TValue value, int index, Action<TValue> configure = default) where TValue : IDataFileValue;

    IDataFileObjectValue WithBlankLinesBetweenItems(bool blankLinesBetweenItems = true);
}