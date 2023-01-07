using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpMetadataBase<TCSharp>
    where TCSharp : CSharpMetadataBase<TCSharp>
{
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    public TCSharp AddMetadata<T>(string key, T value)
    {
        Metadata.Add(key, value);
        return (TCSharp)this;
    }

    public bool HasMetadata(string key)
    {
        return Metadata.ContainsKey(key);
    }

    public T GetMetadata<T>(string key) where T : class
    {
        return Metadata[key] as T;
    }

    public object GetMetadata(string key)
    {
        return Metadata[key];
    }

    public bool TryGetMetadata<T>(string key, out T value)
    {
        if (Metadata.TryGetValue(key, out var valueFound) && valueFound is T castValue)
        {
            value = castValue;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetMetadata(string key, out object value)
    {
        if (Metadata.TryGetValue(key, out var valueFound))
        {
            value = valueFound;
            return true;
        }

        value = null;
        return false;
    }
}