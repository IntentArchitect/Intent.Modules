using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public abstract class IndentedFileItem : IIndentedFileItem
{
    public Dictionary<string, object> Metadata { get; } = new();

    public bool TryGetMetadata<TValue>(string key, out TValue value)
    {
        if (!Metadata.TryGetValue(key, out var objectValue) ||
            objectValue is not TValue castValue)
        {
            value = default;
            return false;
        }

        value = castValue;
        return true;
    }
}