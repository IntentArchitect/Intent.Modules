#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
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
    void AddTypeSource(ITypeSource typeSource);
    ClassTypeSource AddTypeSource(string templateId);
    ClassTypeSource AddTypeSource(string templateId, string collectionFormat);
    void SetDefaultTypeCollectionFormat(string collectionFormat);
    void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter);
    string GetTypeName(IElement typeReference);
    string GetTypeName(ITemplate template, TemplateDiscoveryOptions? options = null);
    string GetTypeName(ITypeReference typeReference, string? collectionFormat = null);
    string GetTypeName(string templateIdOrRole, TemplateDiscoveryOptions? options = null);
    string GetTypeName(string templateIdOrRole, string modelId, TemplateDiscoveryOptions? options = null);
    string GetTypeName(string templateIdOrRole, IMetadataModel model, TemplateDiscoveryOptions? options = null);
    bool TryGetTypeName(string templateId, out string typeName);
    bool TryGetTypeName(string templateId, string modelId, out string typeName);
    bool TryGetTypeName(string templateId, IMetadataModel model, out string typeName);
    TTemplate GetTemplate<TTemplate>(string templateId, TemplateDiscoveryOptions? options = null)
        where TTemplate : class;
    TTemplate GetTemplate<TTemplate>(string templateId, string modelId, TemplateDiscoveryOptions? options = null)
        where TTemplate : class;
    TTemplate GetTemplate<TTemplate>(string templateId, IMetadataModel model, TemplateDiscoveryOptions? options = null)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(string templateId, out TTemplate template)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(string templateId, string modelId, out TTemplate template)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(string templateId, IMetadataModel model, out TTemplate template)
        where TTemplate : class;
    IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference, string? collectionFormat = null);

    /// <summary>
    /// Resolves all found <see cref="IResolvedTypeInfo"/> objects in the order of the supplied <see cref="ITypeSource"/>(s) for the specified <paramref name="typeReference"/>
    /// <see cref="ITypeSource"/>(s) are added to this template with the <see cref="AddTypeSource(Intent.Modules.Common.TypeResolution.ITypeSource)"/> method.
    /// The order in which they are added will determine the order in which this result list is ordered, if those <see cref="ITypeSource"/>(s) resolve a type.
    /// </summary>
    IList<IResolvedTypeInfo> GetAllTypeInfo(ITypeReference typeReference);
}