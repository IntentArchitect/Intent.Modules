using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public interface ITypescriptMetadataBase : ITypescriptCodeContext;

public interface ITypescriptCodeContext
{
    void RepresentsModel(IMetadataModel model);
    ITypescriptCodeContext AddMetadata(string key, object value);
    bool HasMetadata(string key);
    T GetMetadata<T>(string key) where T : class;
    object GetMetadata(string key);
    void RemoveMetadata(string key);
    bool TryGetMetadata<T>(string key, out T value);
    bool TryGetMetadata(string key, out object value);
    IHasTypescriptName GetReferenceForModel(string modelId);
    IHasTypescriptName GetReferenceForModel(IMetadataModel model);
    bool TryGetReferenceForModel(string modelId, out IHasTypescriptName reference);
    bool TryGetReferenceForModel(IMetadataModel model, out IHasTypescriptName reference);
    void RegisterReferenceable(string modelId, ITypescriptReferenceable cSharpReferenceable);
    TypescriptFile File { get; }
    ITypescriptCodeContext Parent { get; }
}

