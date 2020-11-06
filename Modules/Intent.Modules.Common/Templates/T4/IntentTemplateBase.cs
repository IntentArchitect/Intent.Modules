using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentTemplateBase<TModel> : IntentTemplateBase, ITemplateWithModel
    {
        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget)
        {
            Model = model;
        }

        public TModel Model { get; }

        object ITemplateWithModel.Model => Model;

        public override string ToString()
        {
            return $"{Id} [{Model?.ToString()}]";
        }
    }

    public abstract class IntentTemplateBase : T4TemplateBase, ITemplate, IConfigurableTemplate, IHasTemplateDependencies, ITemplatePostConfigurationHook, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        protected readonly ICollection<ITemplateDependency> DetectedDependencies = new List<ITemplateDependency>();

        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget)
        {
            ExecutionContext = outputTarget.ExecutionContext;
            OutputTarget = outputTarget;
            Id = templateId;
            BindingContext = new TemplateBindingContext(this);
        }

        public string Id { get; }
        public ISoftwareFactoryExecutionContext ExecutionContext { get; }
        public IOutputTarget OutputTarget { get; }
        public ITemplateBindingContext BindingContext { get; }
        public IFileMetadata FileMetadata { get; private set; }

        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            FileMetadata = fileMetadata;
        }

        public abstract ITemplateFileConfig GetTemplateFileConfig();

        public virtual string RunTemplate()
        {
            // NOTE: If this method is run multiple times for a template instance, the output is duplicated. Perhaps put in a check here?
            return TransformText();
        }

        public IFileMetadata GetMetadata()
        {
            return FileMetadata;
        }

        public void AddTemplateDependency(string templateId)
        {
            AddTemplateDependency(TemplateDependency.OnTemplate(templateId));
        }

        public void AddTemplateDependency(string templateId, IMetadataModel model)
        {
            AddTemplateDependency(TemplateDependency.OnModel(templateId, model));
        }

        public void AddTemplateDependency(ITemplateDependency templateDependency)
        {
            DetectedDependencies.Add(templateDependency);
        }

        public virtual IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            if (!HasTypeResolver())
            {
                return DetectedDependencies;
            }
            return Types.GetTemplateDependencies().Concat(DetectedDependencies); ;
        }

        public virtual void OnConfigured()
        {
        }

        public virtual void BeforeTemplateExecution()
        {
        }

        private string _defaultTypeCollectionFormat;
        private readonly ICollection<ITypeSource> _typeSources = new List<ITypeSource>();

        public void SetDefaultTypeCollectionFormat(string collectionFormat)
        {
            _defaultTypeCollectionFormat = collectionFormat;
            if (_onCreatedHasHappened)
            {
                Types.DefaultCollectionFormat = collectionFormat;
            }
        }

        public void AddTypeSource(ITypeSource typeSource)
        {
            _typeSources.Add(typeSource);
            if (_onCreatedHasHappened)
            {
                Types.AddClassTypeSource(typeSource);
            }
        }

        private bool _onCreatedHasHappened;

        public virtual void OnCreated()
        {
            _onCreatedHasHappened = true;
            if (!HasTypeResolver())
            {
                return;
            }
            if (_defaultTypeCollectionFormat != null)
            {
                Types.DefaultCollectionFormat = _defaultTypeCollectionFormat;
            }

            foreach (var typeSource in _typeSources)
            {
                Types.AddClassTypeSource(typeSource);
            }
        }

        public virtual string GetTypeName(ITypeReference typeReference, string collectionFormat)
        {
            return Types.Get(typeReference, collectionFormat).Name;
        }

        public virtual string GetTypeName(ITypeReference typeReference)
        {
            return Types.Get(typeReference).Name;
        }

        public virtual string GetTypeName(IHasTypeReference hasTypeReference, string collectionFormat)
        {
            return GetTypeName(hasTypeReference.TypeReference, collectionFormat);
        }

        public virtual string GetTypeName(IHasTypeReference hasTypeReference)
        {
            return GetTypeName(hasTypeReference.TypeReference);
        }

        public virtual string GetTypeName(IElement element, string collectionFormat)
        {
            return Types.Get(element, collectionFormat).Name;
        }

        public virtual string GetTypeName(ICanBeReferencedType element)
        {
            return Types.Get(element).Name;
        }

        public virtual string GetTypeName(ITemplateDependency templateDependency, bool throwIfNotFound = true)
        {
            return GetTemplate<IClassProvider>(templateDependency, throwIfNotFound)?.FullTypeName();
        }

        public string GetTypeName(ITemplate template, bool throwIfNotFound = true)
        {
            return GetTypeName(TemplateDependency.OnTemplate(template), throwIfNotFound);
        }

        public string GetTypeName(string templateId, bool throwIfNotFound = true)
        {
            return GetTypeName(TemplateDependency.OnTemplate(templateId), throwIfNotFound);
        }

        public string GetTypeName(string templateId, IMetadataModel model, bool throwIfNotFound = true)
        {
            return GetTypeName(TemplateDependency.OnModel(templateId, model), throwIfNotFound);
        }

        public string GetTypeName(string templateId, string modelId, bool throwIfNotFound = true)
        {
            return GetTypeName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), throwIfNotFound);
        }

        public TTemplate GetTemplate<TTemplate>(ITemplateDependency dependency)
            where TTemplate : class
        {
            return GetTemplate<TTemplate>(dependency, true);
        }

        public TTemplate GetTemplate<TTemplate>(string templateId) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnTemplate(templateId));
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, IMetadataModel model) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnModel(templateId, model));
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, string modelId) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"));
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, bool throwIfNotFound) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnTemplate(templateId), throwIfNotFound);
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, IMetadataModel model, bool throwIfNotFound) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnModel(templateId, model), throwIfNotFound);
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, string modelId, bool throwIfNotFound) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), throwIfNotFound);
        }

        public TTemplate GetTemplate<TTemplate>(ITemplateDependency dependency, bool throwIfNotFound) where TTemplate : class
        {
            if (!_onCreatedHasHappened)
            {
                throw new Exception($"{nameof(GetTypeName)} cannot be called during template instantiation.");
            }

            var template = ExecutionContext.FindTemplateInstance<TTemplate>(dependency);
            if (template != null)
            {
                DetectedDependencies.Add(dependency);
                return template;
            }

            if (throwIfNotFound)
            {
                throw new Exception($"Could not find template from dependency: {dependency}");
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Id}";
        }
    }

    public class TemplateBindingContext : ITemplateBindingContext
    {
        private object _defaultModelContext;
        private Dictionary<string, object> _prefixLookup;

        public TemplateBindingContext(object defaultModelContext)
        {
            _defaultModelContext = defaultModelContext;
        }

        public TemplateBindingContext() : this(null)
        {
        }

        public void SetDefaultModel(object modelContext)
        {
            _defaultModelContext = modelContext;
        }

        public void AddFakeProperty<T>(string fakePropertyName, T obj)
        {
            if (_prefixLookup == null)
            {
                _prefixLookup = new Dictionary<string, object>();
            }
            _prefixLookup[fakePropertyName] = obj;
        }

        public object GetProperty(string propertyName)
        {
            if (_prefixLookup != null && _prefixLookup.ContainsKey(propertyName))
            {
                return _prefixLookup[propertyName];
            }

            return null;
            //if (_prefixLookup != null && _prefixLookup.ContainsKey(propertyName))
            //{
            //    isDefault = false;
            //    return _prefixLookup[propertyName];
            //}
            //else
            //{
            //    isDefault = true;
            //    return _defaultModelContext;
            //}
        }

        public object GetRootModel()
        {
            return _defaultModelContext;
        }
    }
}
