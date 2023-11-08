using System;
using System.Collections;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public class DataFileArrayValue : DataFileValue<IDataFileArrayValue>, IDataFileArrayValue
{
    private readonly List<IDataFileValue> _values = new();

    public bool BlankLinesBetweenItems { get; set; }

    public IDataFileArrayValue WithArray(Action<IDataFileArrayValue> configure = default) => WithValue(new DataFileArrayValue(), configure);
    public IDataFileArrayValue WithArray(int index, Action<IDataFileArrayValue> configure = default) => WithValue(new DataFileArrayValue(), index, configure);
    public IDataFileArrayValue WithObject(Action<IDataFileObjectValue> configure = default) => WithValue(new DataFileObjectValue(), configure);
    public IDataFileArrayValue WithObject(int index, Action<IDataFileObjectValue> configure = default) => WithValue(new DataFileObjectValue(), index, configure);
    public IDataFileArrayValue WithValue(bool value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(bool value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(byte value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(byte value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(char value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(char value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(decimal value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(decimal value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(double value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(double value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(float value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(float value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(int value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(int value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(long value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(long value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(sbyte value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(sbyte value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(short value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(short value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(string value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(string value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(uint value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(uint value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(ulong value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(ulong value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);
    public IDataFileArrayValue WithValue(ushort value, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, configure);
    public IDataFileArrayValue WithValue(ushort value, int index, Action<IDataFileScalarValue> configure = default) => WithValue((DataFileScalarValue)value, index, configure);

    public IDataFileArrayValue WithValue<TValue>(TValue value, Action<TValue> configure = default) where TValue : IDataFileValue
    {
        return WithValue(value, _values.Count, configure);
    }

    public IDataFileArrayValue WithValue<TValue>(TValue value, int index, Action<TValue> configure = default) where TValue : IDataFileValue
    {
        _values.Insert(index, value);
        OnAdd(value);
        configure?.Invoke(value);

        return this;
    }

    public IDataFileArrayValue WithBlankLinesBetweenItems(bool blankLinesBetweenItems = true)
    {
        BlankLinesBetweenItems = blankLinesBetweenItems;
        return this;
    }

    private void OnAdd(IDataFileValue value)
    {
        value.AttachToParent(Template, this);
    }

    private void OnRemove(IDataFileValue value)
    {
        value.DetachFromParent();
    }

    #region IList<IDataFileValue> implementation

    public IEnumerator<IDataFileValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(IDataFileValue item)
    {
        _values.Add(item);
        OnAdd(item);
    }

    public void Clear()
    {
        var items = _values.ToArray();

        _values.Clear();

        foreach (var item in items)
        {
            OnRemove(item);
        }
    }

    public bool Contains(IDataFileValue item)
    {
        return _values.Contains(item);
    }

    public void CopyTo(IDataFileValue[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    public bool Remove(IDataFileValue item)
    {
        return _values.Remove(item);
    }

    public int Count => _values.Count;

    public bool IsReadOnly => false;

    public int IndexOf(IDataFileValue item)
    {
        return _values.IndexOf(item);
    }

    public void Insert(int index, IDataFileValue item)
    {
        _values.Insert(index, item);
        OnAdd(item);
    }

    public void RemoveAt(int index)
    {
        var item = _values[index];

        _values.RemoveAt(index);
        OnRemove(item);
    }

    public IDataFileValue this[int index]
    {
        get => _values[index];
        set
        {
            var oldValue = _values[index];
            if (ReferenceEquals(oldValue, value))
            {
                return;
            }

            _values[index] = value;

            OnRemove(oldValue);
            OnAdd(value);
        }
    }

    #endregion
}