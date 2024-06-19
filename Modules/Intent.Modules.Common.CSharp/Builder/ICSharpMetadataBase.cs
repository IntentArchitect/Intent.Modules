using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// An alias to have it more aligned with the type implementing it.
/// </summary>
public interface ICSharpMetadataBase : ICSharpCodeContext;

public interface ICSharpCodeContext
{
    public ICSharpCodeContext AddMetadata(string key, object value);
    public bool HasMetadata(string key);
    public T GetMetadata<T>(string key) where T : class;
    public object GetMetadata(string key);
    public bool TryGetMetadata<T>(string key, out T value);
    public bool TryGetMetadata(string key, out object value);
    public IHasCSharpName GetReferenceForModel(string modelId);
    public IHasCSharpName GetReferenceForModel(IMetadataModel model);
    public bool TryGetReferenceForModel(string modelId, out IHasCSharpName reference);
    public bool TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference);
    public void RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable);
    public ICSharpFile File { get; }
}