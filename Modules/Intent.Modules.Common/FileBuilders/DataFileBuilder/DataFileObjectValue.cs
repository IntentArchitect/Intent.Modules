using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public class DataFileObjectValue : DataFileValue<IDataFileObjectValue>, IDataFileObjectValue
{
    private readonly OrderedDictionary _dictionary = new();

    public bool BlankLinesBetweenItems { get; set; }

    public IDataFileObjectValue WithArray(string name, Action<IDataFileArrayValue> configure = default) => WithValue(name, new DataFileArrayValue(), configure);
    public IDataFileObjectValue WithArray(string name, int index, Action<IDataFileArrayValue> configure = default) => WithValue(name, new DataFileArrayValue(), index, configure);
    public IDataFileObjectValue WithObject(string name, Action<IDataFileObjectValue> configure = default) => WithValue(name, new DataFileObjectValue(), configure);
    public IDataFileObjectValue WithObject(string name, int index, Action<IDataFileObjectValue> configure = default) => WithValue(name, new DataFileObjectValue(), index, configure);
    public IDataFileObjectValue WithValue(string name, bool value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, bool value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, byte value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, byte value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, char value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, char value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, decimal value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, decimal value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, double value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, double value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, float value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, float value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, int value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, int value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, long value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, long value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, sbyte value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, sbyte value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, short value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, short value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, string value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, string value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, uint value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, uint value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, ulong value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, ulong value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);
    public IDataFileObjectValue WithValue(string name, ushort value, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, configure);
    public IDataFileObjectValue WithValue(string name, ushort value, int index, Action<IDataFileScalarValue> configure = default) => WithValue(name, (DataFileScalarValue)value, index, configure);

    public IDataFileObjectValue WithValue<TValue>(string name, TValue value, Action<TValue> configure = default)
        where TValue : IDataFileValue
    {
        return WithValue(name, value, _dictionary.Count, configure);
    }

    public IDataFileObjectValue WithValue<TValue>(string name, TValue value, int index, Action<TValue> configure = default)
        where TValue : IDataFileValue
    {
        _dictionary.Insert(index, name, value);
        OnAdd(value);
        configure?.Invoke(value);

        return this;
    }

    public IDataFileObjectValue WithBlankLinesBetweenItems(bool blankLinesBetweenItems = true)
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

    #region IDictionary<string, DataFileValueType> implementation

    IEnumerator<KeyValuePair<string, DataFileValue>> IEnumerable<KeyValuePair<string, DataFileValue>>.GetEnumerator() => _dictionary
        .Cast<DictionaryEntry>()
        .Select(x => new KeyValuePair<string, DataFileValue>((string)x.Key, (DataFileValue)x.Value))
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<string, DataFileValue>)this).GetEnumerator();
    }

    void ICollection<KeyValuePair<string, DataFileValue>>.Add(KeyValuePair<string, DataFileValue> item)
    {
        _dictionary.Add(item.Key, item.Value);
        OnAdd(item.Value);
    }

    void ICollection<KeyValuePair<string, DataFileValue>>.Clear()
    {
        var items = this.ToArray();

        _dictionary.Clear();

        foreach (var item in items)
        {
            OnRemove(item.Value);
        }
    }

    bool ICollection<KeyValuePair<string, DataFileValue>>.Contains(KeyValuePair<string, DataFileValue> item)
    {
        return _dictionary.Contains(item.Key) && ReferenceEquals(_dictionary[item.Key], item.Value);
    }

    void ICollection<KeyValuePair<string, DataFileValue>>.CopyTo(KeyValuePair<string, DataFileValue>[] array, int arrayIndex)
    {
        this.ToArray().CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<string, DataFileValue>>.Remove(KeyValuePair<string, DataFileValue> item)
    {
        if (!((IDictionary<string, DataFileValue>)this).Contains(item) ||
            !ReferenceEquals(_dictionary[item.Key], item.Value))
        {
            return false;
        }

        _dictionary.Remove(item.Key);
        OnRemove(item.Value);
        return true;
    }

    int ICollection<KeyValuePair<string, DataFileValue>>.Count => _dictionary.Count;

    bool ICollection<KeyValuePair<string, DataFileValue>>.IsReadOnly => false;

    void IDictionary<string, DataFileValue>.Add(string key, DataFileValue value)
    {
        _dictionary.Add(key, value);
        OnAdd(value);
    }

    bool IDictionary<string, DataFileValue>.ContainsKey(string key)
    {
        return _dictionary.Contains(key);
    }

    bool IDictionary<string, DataFileValue>.Remove(string key)
    {
        if (_dictionary.Contains(key))
        {
            return false;
        }

        var value = (DataFileValue)_dictionary[key];

        _dictionary.Remove(key);
        OnRemove(value);
        return true;
    }

    bool IDictionary<string, DataFileValue>.TryGetValue(string key, out DataFileValue value)
    {
        if (_dictionary.Contains(key))
        {
            value = default;
            return false;
        }

        value = (DataFileValue)_dictionary[key];
        return true;
    }

    DataFileValue IDictionary<string, DataFileValue>.this[string key]
    {
        get => (DataFileValue)_dictionary[key];
        set
        {
            var exists = ((IDictionary<string, DataFileValue>)this).TryGetValue(key, out var existingValue);
            if (exists && ReferenceEquals(existingValue, value))
            {
                return;
            }

            _dictionary[key] = value;
            if (exists)
            {
                OnRemove(existingValue);
            }

            OnAdd(value);
        }
    }

    ICollection<string> IDictionary<string, DataFileValue>.Keys => _dictionary.Keys.Cast<string>().ToList();

    ICollection<DataFileValue> IDictionary<string, DataFileValue>.Values => _dictionary.Values.Cast<DataFileValue>().ToList();

    #endregion
}
