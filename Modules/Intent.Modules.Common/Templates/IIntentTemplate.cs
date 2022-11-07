using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Templates;

public interface IIntentTemplate<out T> : IIntentTemplate
{
    T Model { get; }
}

public interface IIntentTemplate : ITemplate
{
    ISoftwareFactoryExecutionContext ExecutionContext { get; }
    IOutputTarget OutputTarget { get; }
    IFileMetadata FileMetadata { get; }
    void FulfillsRole(string role);
    string GetTypeName(ITypeReference typeReference, string collectionFormat = null);
    string GetTypeName(string templateIdOrRole, TemplateDiscoveryOptions options = null);
    string GetTypeName(string templateIdOrRole, string modelId, TemplateDiscoveryOptions options = null);
    string GetTypeName(string templateIdOrRole, IMetadataModel model, TemplateDiscoveryOptions options = null);
    IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference, string collectionFormat = null);
}