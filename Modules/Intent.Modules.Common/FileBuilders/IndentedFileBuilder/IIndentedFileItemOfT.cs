namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public interface IIndentedFileItem<out T> : IIndentedFileItem
    where T : IIndentedFileItem<T>
{
    T AddMetadata(string key, object value);
}