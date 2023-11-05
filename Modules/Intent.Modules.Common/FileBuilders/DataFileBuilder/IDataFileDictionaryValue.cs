using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileDictionaryValue : IDataFileValue<IDataFileDictionaryValue>, IDictionary<string, DataFileValue>
{
    bool BlankLinesBetweenItems { get; set; }
    IDataFileDictionaryValue WithArray(string name, Action<IDataFileArrayValue> configure = default);
    IDataFileDictionaryValue WithArray(string name, int index, Action<IDataFileArrayValue> configure = default);
    IDataFileDictionaryValue WithDictionary(string name, Action<IDataFileDictionaryValue> configure = default);
    IDataFileDictionaryValue WithDictionary(string name, int index, Action<IDataFileDictionaryValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, bool value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, bool value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, byte value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, byte value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, char value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, char value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, decimal value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, decimal value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, double value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, double value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, float value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, float value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, int value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, int value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, long value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, long value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, sbyte value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, sbyte value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, short value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, short value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, string value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, string value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, uint value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, uint value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, ulong value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, ulong value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, ushort value, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue(string name, ushort value, int index, Action<IDataFileScalarValue> configure = default);
    IDataFileDictionaryValue WithValue<TValue>(string name, TValue value, Action<TValue> configure = default) where TValue : IDataFileValue;
    IDataFileDictionaryValue WithValue<TValue>(string name, TValue value, int index, Action<TValue> configure = default) where TValue : IDataFileValue;

    IDataFileDictionaryValue WithBlankLinesBetweenItems(bool blankLinesBetweenItems = true);
}