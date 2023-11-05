using System;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public abstract class IndentedFileItem<T> : IndentedFileItem, IIndentedFileItem<T>
    where T : class, IIndentedFileItem<T>
{
    public T AddMetadata(string key, object value)
    {
        Metadata.Add(key, value);
        return (T)this;
    }

    public static explicit operator T(IndentedFileItem<T> t) => t as T ?? throw new InvalidOperationException();
}