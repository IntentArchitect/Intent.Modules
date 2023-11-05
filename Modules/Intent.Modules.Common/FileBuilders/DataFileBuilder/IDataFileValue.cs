using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public interface IDataFileValue
{
    IDataFileValue Parent { get; }
    string Comment { get; set; }
    bool IsCommentedOut { get; set; }
    Dictionary<string, object> Metadata { get; }
    bool TryGetMetadata<T>(string key, out T value);
    void AttachToParent(IDataFileBuilderTemplate template, IDataFileValue parent);
    void DetachFromParent();
}