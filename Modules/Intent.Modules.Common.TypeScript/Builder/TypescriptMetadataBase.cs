using Intent.Metadata.Models;
using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public abstract class TypescriptMetadataBase : ITypescriptMetadataBase
{
    private readonly Dictionary<string, ITypescriptReferenceable> _modelReferenceRegistry = new();

    public ITypescriptCodeContext Parent { get; set; }
    public TypescriptFile File { get; protected set; }
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    public void RegisterReferenceable(string modelId, ITypescriptReferenceable reference)
    {
        if (_modelReferenceRegistry.ContainsKey(modelId))
        {
            throw new InvalidOperationException($"Cannot add Typescript reference '{reference.Name}' for the model with id {modelId}: A reference with this key already exists '{GetReferenceForModel(modelId).Name}'.");
        }
        _modelReferenceRegistry.Add(modelId, reference);
    }

    public ITypescriptCodeContext AddMetadata(string key, object value)
    {
        Metadata.Add(key, value);
        return this;
    }
    public bool HasMetadata(string key)
    {
        return Metadata.ContainsKey(key);
    }

    public T GetMetadata<T>(string key) where T : class
    {
        return Metadata[key] as T;
    }

    public object GetMetadata(string key)
    {
        return Metadata[key];
    }

    public void RemoveMetadata(string key)
    {
        Metadata.Remove(key);
    }

    public bool TryGetMetadata<T>(string key, out T value)
    {
        if (Metadata.TryGetValue(key, out var valueFound) && valueFound is T castValue)
        {
            value = castValue;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetMetadata(string key, out object value)
    {
        if (Metadata.TryGetValue(key, out var valueFound))
        {
            value = valueFound;
            return true;
        }

        value = null;
        return false;
    }

    public IHasTypescriptName GetReferenceForModel(string modelId)
    {
        return TryGetReferenceForModel(modelId, out var reference)
               ? reference
               : throw new Exception($"Could not find reference for model [${modelId}] on [{GetType().Name}]");
    }

    public IHasTypescriptName GetReferenceForModel(IMetadataModel model)
    {
        return GetReferenceForModel(model.Id);
    }

    public bool TryGetReferenceForModel(string modelId, out IHasTypescriptName reference)
    {
        reference = _modelReferenceRegistry.ContainsKey(modelId)
            ? _modelReferenceRegistry[modelId]
            : null;
        if (reference == null)
        {
            return Parent?.TryGetReferenceForModel(modelId, out reference) ?? false;
        }

        return true;
    }

    public bool TryGetReferenceForModel(IMetadataModel model, out IHasTypescriptName reference)
    {
        return TryGetReferenceForModel(model.Id, out reference);
    }

    public void RepresentsModel(IMetadataModel model)
    {
        if (this is not IHasTypescriptName)
        {
            throw new InvalidOperationException($"This functionality is not supported on this type: {GetType().Name}");
        }

        if (Parent == null)
        {
            throw new InvalidOperationException($"The parent has not been set for this type: {GetType().Name}. Please contact Intent Architect support.");
        }

        Parent.RegisterReferenceable(model.Id, (ITypescriptReferenceable)this);
    }
}

public abstract class TypescriptMetadataBase<TTypescript> : TypescriptMetadataBase
    where TTypescript : TypescriptMetadataBase<TTypescript>
{
    public IMetadataModel RepresentedModel { get; private set; }

    public new TTypescript RepresentsModel(IMetadataModel model)
    {
        base.RepresentsModel(model);
        RepresentedModel = model;
        return (TTypescript)this;
    }

    public TTypescript AddMetadata<T>(string key, T value)
    {
        Metadata.Add(key, value);
        return (TTypescript)this;
    }
}