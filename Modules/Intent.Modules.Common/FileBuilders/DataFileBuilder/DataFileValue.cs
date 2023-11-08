using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder;

public abstract class DataFileValue : IDataFileValue
{
    public IDataFileBuilderTemplate Template { get; private set; }

    public IDataFileValue Parent { get; private set; }

    public string Comment { get; set; }

    public bool IsCommentedOut { get; set; }

    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    public bool TryGetMetadata<T>(string key, out T value)
    {
        if (!Metadata.TryGetValue(key, out var objectValue) ||
            objectValue is not T castValue)
        {
            value = default;
            return false;
        }

        value = castValue;
        return true;
    }

    public void AttachToParent(IDataFileBuilderTemplate template, IDataFileValue parent)
    {
        Template = template;
        Parent = parent;
    }

    public void DetachFromParent()
    {
        Template = null;
        Parent = null;
    }

    public void Accept(IDataFileValueVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override string ToString()
    {
        var writer = Template.DataFile.WriterProvider();
        Accept(writer);
        return writer.ToString();
    }
}