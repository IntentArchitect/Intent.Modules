using System;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public abstract class DataFileValue<T> : DataFileValue, IDataFileValue<T>
    where T : class, IDataFileValue<T>
{
    public T WithComment(string comment)
    {
        Comment = comment;
        return (T)this;
    }

    public T WithMetadata(string key, object value)
    {
        Metadata.Add(key, value);
        return (T)this;
    }

    public T CommentedOut(bool isCommentedOut = true)
    {
        IsCommentedOut = isCommentedOut;
        return (T)this;
    }

    public static explicit operator T(DataFileValue<T> t) => t as T ?? throw new InvalidOperationException();
}