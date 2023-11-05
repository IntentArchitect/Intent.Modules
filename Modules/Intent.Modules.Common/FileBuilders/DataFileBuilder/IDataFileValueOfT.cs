namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileValue<out T> : IDataFileValue
    where T : IDataFileValue<T>
{
    T WithComment(string comment);
    T WithMetadata(string key, object value);
    T CommentedOut(bool isCommentedOut = true);
}