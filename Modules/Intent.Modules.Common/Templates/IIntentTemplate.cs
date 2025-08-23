#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Templates;

public interface IIntentTemplate<out T> : IIntentTemplate
{
    T? Model { get; }
}

public interface IIntentTemplate : ITemplate
{
    bool CanRun { get => true; set { } }
    bool IsDiscoverable { get => true; set { } }
    ISoftwareFactoryExecutionContext ExecutionContext { get; }
    IOutputTarget OutputTarget { get; }
    IFileMetadata FileMetadata { get; }
    void FulfillsRole(string role);
    void AddTypeSource(ITypeSource typeSource);
    ClassTypeSource AddTypeSource(string templateId);
    ClassTypeSource AddTypeSource(string templateId, string collectionFormat);
    void SetDefaultTypeCollectionFormat(string collectionFormat);
    void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter);

    string? GetTypeName(IElement typeReference);
    string? GetTypeName(ITemplate template, TemplateDiscoveryOptions? options = null);
    string? GetTypeName(ITypeReference typeReference, string? collectionFormat = null);
    string? GetTypeName(string templateIdOrRole, TemplateDiscoveryOptions? options = null);
    string? GetTypeName(string templateIdOrRole, string modelId, TemplateDiscoveryOptions? options = null);
    string? GetTypeName(string templateIdOrRole, IMetadataModel model, TemplateDiscoveryOptions? options = null);

    /// <summary>
    /// Tries to get existing file content of this template's output.
    /// </summary>
    /// <remarks>
    /// This method takes into account that the output path may have changed since the previous
    /// Software Factory execution.
    /// </remarks>
    /// <param name="content">The contents of the file if it exists.</param>
    /// <returns>whether there was an existing file for this template's output.</returns>
    bool TryGetExistingFileContent([NotNullWhen(true)] out string? content)
    {
        Logging.Log.Warning($"{GetType()} does not have an implementation for TryGetExistingFileContent(string)");
        content = null!;
        return false;
    }

    /// <summary>
    /// If an existing file exists, returns <see langword="true"/> and populates the
    /// <paramref name="path"/> with the existing file's path.
    /// </summary>
    /// <remarks>
    /// At the end of a software factory execution a template's output path is recorded in a
    /// log and this method reads the log to determine what the previous output path was.
    /// <para>
    /// Regardless of whether the current output path is different compared to the
    /// previous software factory execution, if a file exists at the current output path, then
    /// the current output path is populated into the <paramref name="path"/> parameter.
    /// </para>
    /// <para>
    /// If no file exists at the current output path, then the previous output path is checked
    /// to see if it exists.
    /// </para>
    /// </remarks>
    bool TryGetExistingFilePath([NotNullWhen(true)] out string? path)
    {
        Logging.Log.Warning($"{GetType()} does not have an implementation for TryGetExistingFilePath(string)");
        path = null!;
        return false;
    }

    bool TryGetTypeName(string templateId, [NotNullWhen(true)] out string? typeName);
    bool TryGetTypeName(string templateId, string modelId, [NotNullWhen(true)] out string? typeName);
    bool TryGetTypeName(string templateId, IMetadataModel model, [NotNullWhen(true)] out string? typeName);
    bool TryGetTypeName(IEnumerable<string> templateIds, string modelId, [NotNullWhen(true)] out string? typeName);
    bool TryGetTypeName(IEnumerable<string> templateIds, IMetadataModel model, [NotNullWhen(true)] out string? typeName);
    TTemplate? GetTemplate<TTemplate>(string templateId, TemplateDiscoveryOptions? options = null)
        where TTemplate : class;
    TTemplate? GetTemplate<TTemplate>(string templateId, string modelId, TemplateDiscoveryOptions? options = null)
        where TTemplate : class;
    TTemplate? GetTemplate<TTemplate>(string templateId, IMetadataModel model, TemplateDiscoveryOptions? options = null)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(string templateId, [NotNullWhen(true)] out TTemplate? template)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(string templateId, string modelId, [NotNullWhen(true)] out TTemplate? template)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(string templateId, IMetadataModel model, [NotNullWhen(true)] out TTemplate? template)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(IEnumerable<string> templateIds, string modelId, [NotNullWhen(true)] out TTemplate? template)
        where TTemplate : class;
    bool TryGetTemplate<TTemplate>(IEnumerable<string> templateIds, IMetadataModel model, [NotNullWhen(true)] out TTemplate? template)
        where TTemplate : class;
    IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference, string? collectionFormat = null);

    /// <summary>
    /// Resolves all found <see cref="IResolvedTypeInfo"/> objects in the order of the supplied <see cref="ITypeSource"/>(s) for the specified <paramref name="typeReference"/>
    /// <see cref="ITypeSource"/>(s) are added to this template with the <see cref="AddTypeSource(Intent.Modules.Common.TypeResolution.ITypeSource)"/> method.
    /// The order in which they are added will determine the order in which this result list is ordered, if those <see cref="ITypeSource"/>(s) resolve a type.
    /// </summary>
    IList<IResolvedTypeInfo> GetAllTypeInfo(ITypeReference typeReference);
}