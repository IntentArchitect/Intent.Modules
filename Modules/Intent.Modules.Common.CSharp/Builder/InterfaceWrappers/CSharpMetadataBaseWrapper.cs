using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal abstract class CSharpMetadataBaseWrapper(ICSharpMetadataBase wrapped) : ICSharpMetadataBase
{
    ICSharpCodeContext ICSharpCodeContext.AddMetadata(string key, object value) => wrapped.AddMetadata(key, value);

    bool ICSharpCodeContext.HasMetadata(string key) => wrapped.HasMetadata(key);

    T ICSharpCodeContext.GetMetadata<T>(string key) => wrapped.GetMetadata<T>(key);

    object ICSharpCodeContext.GetMetadata(string key) => wrapped.GetMetadata(key);

    public void RemoveMetadata(string key) => wrapped.GetMetadata(key);

    bool ICSharpCodeContext.TryGetMetadata<T>(string key, out T value) => wrapped.TryGetMetadata(key, out value);

    bool ICSharpCodeContext.TryGetMetadata(string key, out object value) => wrapped.TryGetMetadata(key, out value);

    IHasCSharpName ICSharpCodeContext.GetReferenceForModel(string modelId) => wrapped.GetReferenceForModel(modelId);

    IHasCSharpName ICSharpCodeContext.GetReferenceForModel(IMetadataModel model) => wrapped.GetReferenceForModel(model);

    bool ICSharpCodeContext.TryGetReferenceForModel(string modelId, out IHasCSharpName reference) => wrapped.TryGetReferenceForModel(modelId, out reference);

    bool ICSharpCodeContext.TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference) => wrapped.TryGetReferenceForModel(model, out reference);

    void ICSharpCodeContext.RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable) => wrapped.RegisterReferenceable(modelId, cSharpReferenceable);

    ICSharpFile ICSharpCodeContext.File => wrapped.File;
    public ICSharpCodeContext Parent => wrapped.Parent;

    void ICSharpCodeContext.RepresentsModel(IMetadataModel model) => wrapped.RepresentsModel(model);
}