using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpCodeContext
{
    ICSharpCodeContext AddMetadata(string key, object value);
    public bool HasMetadata(string key);
    public T GetMetadata<T>(string key) where T : class;
    public object GetMetadata(string key);
    public bool TryGetMetadata<T>(string key, out T value);
    public bool TryGetMetadata(string key, out object value);
    IHasCSharpName GetReferenceForModel(string modelId);
    IHasCSharpName GetReferenceForModel(IMetadataModel model);
    bool TryGetReferenceForModel(string modelId, out IHasCSharpName reference);
    bool TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference);
    void RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable);
    ICSharpFile File { get; }
}