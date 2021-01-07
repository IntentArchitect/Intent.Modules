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
    public abstract class IntentTemplateBase<TModel, TDecorator> : IntentTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        public void AddDecorator(TDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc)
        {
            return GetDecorators().Aggregate(propertyFunc);
        }
    }

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

        #region GetTypeName for TypeReference

        /// <summary>
        /// Override this to alter Type names after they have been found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string NormalizeTypeName(string name)
        {
            return name;
        }

        public virtual IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference)
        {
            return Types.Get(typeReference);
        }

        public virtual string GetTypeName(ITypeReference typeReference, string collectionFormat)
        {
            return NormalizeTypeName(Types.Get(typeReference, collectionFormat).Name);
        }

        public virtual string GetTypeName(ITypeReference typeReference)
        {
            return NormalizeTypeName(Types.Get(typeReference).Name);
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
            return NormalizeTypeName(Types.Get(element, collectionFormat).Name);
        }

        public virtual string GetTypeName(ICanBeReferencedType element)
        {
            return NormalizeTypeName(Types.Get(element).Name);
        }

        #endregion
        #region GetTypeName for Template

        public virtual string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            var name = GetTemplate<IClassProvider>(templateDependency, options)?.FullTypeName();
            return name != null ? NormalizeTypeName(name) : null;
        }

        public string GetTypeName(ITemplate template, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnTemplate(template), options);
        }

        public string GetTypeName(string templateId, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnTemplate(templateId), options);
        }

        public string GetTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnModel(templateId, model), options);
        }

        public string GetTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), options);
        }

        #endregion
        #region GetTemplate

        public TTemplate GetTemplate<TTemplate>(string templateId, TemplateDiscoveryOptions options = null) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnTemplate(templateId), options);
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnModel(templateId, model), options);
        }

        public TTemplate GetTemplate<TTemplate>(string templateId, string modelId, TemplateDiscoveryOptions options = null) where TTemplate : class
        {
            return GetTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), options);
        }

        public TTemplate GetTemplate<TTemplate>(ITemplateDependency dependency, TemplateDiscoveryOptions options = null) where TTemplate : class
        {
            if (options == null)
            {
                options = new TemplateDiscoveryOptions();
            }
            if (!_onCreatedHasHappened)
            {
                throw new Exception($"{nameof(GetTypeName)} cannot be called during template instantiation.");
            }

            var template = ExecutionContext.FindTemplateInstance<TTemplate>(dependency);

            if (template == null && options.ThrowIfNotFound)
            {
                throw new Exception($"Could not find template from dependency: {dependency}");
            }

            if (options.TrackDependency && template != null)
            {
                DetectedDependencies.Add(dependency);
                return template;
            }

            return template;
        }

        #endregion

        public override string ToString()
        {
            return $"{Id}";
        }
    }

    public class TemplateDiscoveryOptions
    {
        public bool ThrowIfNotFound { get; set; } = true;
        public bool TrackDependency { get; set; } = true;
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
