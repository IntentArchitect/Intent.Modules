using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal abstract class CSharpMetadataBaseWrapper(ICSharpMetadataBase wrapped) : ICSharpMetadataBase
{
    public ICSharpCodeContext AddMetadata(string key, object value)
    {
        return wrapped.AddMetadata(key, value);
    }

    public bool HasMetadata(string key)
    {
        return wrapped.HasMetadata(key);
    }

    public T GetMetadata<T>(string key) where T : class
    {
        return wrapped.GetMetadata<T>(key);
    }

    public object GetMetadata(string key)
    {
        return wrapped.GetMetadata(key);
    }

    public bool TryGetMetadata<T>(string key, out T value)
    {
        return wrapped.TryGetMetadata(key, out value);
    }

    public bool TryGetMetadata(string key, out object value)
    {
        return wrapped.TryGetMetadata(key, out value);
    }

    public IHasCSharpName GetReferenceForModel(string modelId)
    {
        return wrapped.GetReferenceForModel(modelId);
    }

    public IHasCSharpName GetReferenceForModel(IMetadataModel model)
    {
        return wrapped.GetReferenceForModel(model);
    }

    public bool TryGetReferenceForModel(string modelId, out IHasCSharpName reference)
    {
        return wrapped.TryGetReferenceForModel(modelId, out reference);
    }

    public bool TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference)
    {
        return wrapped.TryGetReferenceForModel(model, out reference);
    }

    public void RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable)
    {
        wrapped.RegisterReferenceable(modelId, cSharpReferenceable);
    }

    public ICSharpFile File => wrapped.File;
}