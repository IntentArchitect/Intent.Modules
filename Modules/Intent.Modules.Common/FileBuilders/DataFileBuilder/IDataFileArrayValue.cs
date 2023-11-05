using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileArrayValue : IDataFileValue<IDataFileArrayValue>, IList<IDataFileValue>
{
    bool BlankLinesBetweenItems { get; set; }

    IDataFileArrayValue WithArray(Action<IDataFileArrayValue> configure = default);
    IDataFileArrayValue WithArray(int index, Action<IDataFileArrayValue> configure = default);
    IDataFileArrayValue WithDictionary(Action<IDataFileDictionaryValue> configure = default);
    IDataFileArrayValue WithDictionary(int index, Action<IDataFileDictionaryValue> configure = default);
    IDataFileArrayValue WithValue(bool value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(bool value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(byte value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(byte value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(char value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(char value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(decimal value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(decimal value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(double value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(double value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(float value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(float value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(int value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(int value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(long value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(long value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(sbyte value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(sbyte value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(short value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(short value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(string value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(string value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(uint value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(uint value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(ulong value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(ulong value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(ushort value, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue(ushort value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileArrayValue WithValue<TValue>(TValue value, Action<TValue> configure = default) where TValue : IDataFileValue;
    IDataFileArrayValue WithValue<TValue>(TValue value, int index, Action<TValue> configure = default) where TValue : IDataFileValue;

    IDataFileArrayValue WithBlankLinesBetweenItems(bool blankLinesBetweenItems = true);
}