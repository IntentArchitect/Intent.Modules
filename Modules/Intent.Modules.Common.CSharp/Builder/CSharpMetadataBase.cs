using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpMetadataBase : ICSharpMetadataBase
{
    private readonly Dictionary<string, ICSharpReferenceable> _modelReferenceRegistry = new();

    public ICSharpCodeContext Parent { get; set; }
    public ICSharpFile File { get; protected set; }
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    public void RegisterReferenceable(string modelId, ICSharpReferenceable reference)
    {
        if (_modelReferenceRegistry.ContainsKey(modelId))
        {
            throw new InvalidOperationException($"Cannot add CSharp reference '{reference.Name}' for the model with id {modelId}: A reference with this key already exists '{GetReferenceForModel(modelId).Name}'.");
        }
        _modelReferenceRegistry.Add(modelId, reference);
    }

    public ICSharpCodeContext AddMetadata(string key, object value)
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

    public IHasCSharpName GetReferenceForModel(string modelId)
    {
        return TryGetReferenceForModel(modelId, out var reference)
               ? reference
               : throw new Exception($"Could not find reference for model [${modelId}] on [{GetType().Name}]");
    }

    public IHasCSharpName GetReferenceForModel(IMetadataModel model)
    {
        return GetReferenceForModel(model.Id);
    }

    public bool TryGetReferenceForModel(string modelId, out IHasCSharpName reference)
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

    public bool TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference)
    {
        return TryGetReferenceForModel(model.Id, out reference);
    }

    public void RepresentsModel(IMetadataModel model)
    {
        if (this is not IHasCSharpName)
        {
            throw new InvalidOperationException($"This functionality is not supported on this type: {GetType().Name}");
        }

        if (Parent == null)
        {
            throw new InvalidOperationException($"The parent has not been set for this type: {GetType().Name}. Please contact Intent Architect support.");
        }

        Parent.RegisterReferenceable(model.Id, (ICSharpReferenceable)this);
    }
}

public abstract class CSharpMetadataBase<TCSharp> : CSharpMetadataBase
    where TCSharp : CSharpMetadataBase<TCSharp>
{
    /// <summary>
    /// This is used to store the model for which the element represents
    /// This is stored so that it can be used to make more context aware decisions
    /// when generating the code
    /// </summary>
    public IMetadataModel RepresentedModel { get; private set; }

    public new TCSharp RepresentsModel(IMetadataModel model)
    {
        base.RepresentsModel(model);
        RepresentedModel = model;
        return (TCSharp)this;
    }

    public TCSharp AddMetadata<T>(string key, T value)
    {
        Metadata.Add(key, value);
        return (TCSharp)this;
    }
}
