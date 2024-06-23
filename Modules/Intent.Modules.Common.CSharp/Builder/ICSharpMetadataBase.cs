using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// An alias to have it more aligned with the type implementing it.
/// </summary>
public interface ICSharpMetadataBase : ICSharpCodeContext;

public interface ICSharpCodeContext
{
    void RepresentsModel(IMetadataModel model);
    ICSharpCodeContext AddMetadata(string key, object value);
    bool HasMetadata(string key);
    T GetMetadata<T>(string key) where T : class;
    object GetMetadata(string key);
    bool TryGetMetadata<T>(string key, out T value);
    bool TryGetMetadata(string key, out object value);
    IHasCSharpName GetReferenceForModel(string modelId);
    IHasCSharpName GetReferenceForModel(IMetadataModel model);
    bool TryGetReferenceForModel(string modelId, out IHasCSharpName reference);
    bool TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference);
    void RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable);
    ICSharpFile File { get; }
    ICSharpCodeContext Parent { get; }
}