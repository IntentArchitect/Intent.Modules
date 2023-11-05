using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public interface IIndentedFileItem
{
    Dictionary<string, object> Metadata { get; }
    bool TryGetMetadata<TValue>(string key, out TValue value);
}