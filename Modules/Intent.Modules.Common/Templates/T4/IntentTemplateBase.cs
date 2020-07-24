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
        protected IntentTemplateBase(string templateId, ITemplateExecutionContext executionContext, TModel model) : base(templateId, executionContext)
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

        protected IntentTemplateBase(string templateId, ITemplateExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
            OutputTarget = executionContext;
            Id = templateId;
            BindingContext = new TemplateBindingContext(this);
        }

        public string Id { get; }
        public ITemplateExecutionContext ExecutionContext { get; }
        public IOutputTarget OutputTarget { get; }
        public ITemplateBindingContext BindingContext { get; }
        public IFileMetadata FileMetadata { get; private set; }

        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            FileMetadata = fileMetadata;
        }


        public abstract ITemplateFileConfig DefineDefaultFileMetadata();

        public virtual string RunTemplate()
        {
            // NOTE: If this method is run multiple times for a template instance, the output is duplicated. Perhaps put in a check here?
            return TransformText();
        }

        public IFileMetadata GetMetadata()
        {
            return FileMetadata;
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
        private readonly ICollection<IClassTypeSource> _typeSources = new List<IClassTypeSource>();

        public void SetDefaultTypeCollectionFormat(string collectionFormat)
        {
            _defaultTypeCollectionFormat = collectionFormat;
            if (_onCreatedHasHappened)
            {
                Types.DefaultCollectionFormat = collectionFormat;
            }
        }

        public void AddTypeSource(IClassTypeSource classTypeSource)
        {
            _typeSources.Add(classTypeSource);
            if (_onCreatedHasHappened)
            {
                Types.AddClassTypeSource(classTypeSource);
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
            return Types.Get(typeReference, collectionFormat);
        }

        public virtual string GetTypeName(ITypeReference typeReference)
        {
            return Types.Get(typeReference);
        }

        public virtual string GetTemplateClassName(ITemplateDependency templateDependency)
        {
            return FindTemplate<IHasClassDetails>(templateDependency).FullTypeName();
        }

        public string GetTemplateClassName(ITemplate template)
        {
            return GetTemplateClassName(TemplateDependency.OnTemplate(template));
        }

        public string GetTemplateClassName(string templateId)
        {
            return GetTemplateClassName(TemplateDependency.OnTemplate(templateId));
        }

        public string GetTemplateClassName(string templateId, IMetadataModel model)
        {
            return GetTemplateClassName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == model.Id, model));
        }

        public string GetTemplateClassName(string templateId, string modelId)
        {
            return GetTemplateClassName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"));
        }

        public virtual string GetTemplateClassName(ITemplateDependency templateDependency, bool throwIfNotFound)
        {
            return FindTemplate<IHasClassDetails>(templateDependency, throwIfNotFound).FullTypeName();
        }

        public string GetTemplateClassName(ITemplate template, bool throwIfNotFound)
        {
            return GetTemplateClassName(TemplateDependency.OnTemplate(template), throwIfNotFound);
        }

        public string GetTemplateClassName(string templateId, bool throwIfNotFound)
        {
            return GetTemplateClassName(TemplateDependency.OnTemplate(templateId), throwIfNotFound);
        }

        public string GetTemplateClassName(string templateId, IMetadataModel model, bool throwIfNotFound)
        {
            return GetTemplateClassName(TemplateDependency.OnModel(templateId, model), throwIfNotFound);
        }

        public string GetTemplateClassName(string templateId, string modelId, bool throwIfNotFound)
        {
            return GetTemplateClassName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), throwIfNotFound);
        }

        public TTemplate FindTemplate<TTemplate>(ITemplateDependency dependency)
            where TTemplate : class
        {
            return FindTemplate<TTemplate>(dependency, true);
        }

        public TTemplate FindTemplate<TTemplate>(string templateId) where TTemplate : class
        {
            return FindTemplate<TTemplate>(TemplateDependency.OnTemplate(templateId));
        }

        public TTemplate FindTemplate<TTemplate>(string templateId, IMetadataModel model) where TTemplate : class
        {
            return FindTemplate<TTemplate>(TemplateDependency.OnModel(templateId, model));
        }

        public TTemplate FindTemplate<TTemplate>(string templateId, string modelId) where TTemplate : class
        {
            return FindTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"));
        }

        public TTemplate FindTemplate<TTemplate>(ITemplateDependency dependency, bool throwIfNotFound) where TTemplate : class
        {
            if (!_onCreatedHasHappened)
            {
                throw new Exception($"${nameof(GetTemplateClassName)} cannot be called during template instantiation.");
            }

            var template = ExecutionContext.Application.FindTemplateInstance<TTemplate>(dependency);
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

        public TTemplate FindTemplate<TTemplate>(string templateId, bool throwIfNotFound = true) where TTemplate : class
        {
            return FindTemplate<TTemplate>(TemplateDependency.OnTemplate(templateId), throwIfNotFound);
        }

        public TTemplate FindTemplate<TTemplate>(string templateId, IMetadataModel model, bool throwIfNotFound = true) where TTemplate : class
        {
            return FindTemplate<TTemplate>(TemplateDependency.OnModel(templateId, model), throwIfNotFound);
        }

        public TTemplate FindTemplate<TTemplate>(string templateId, string modelId, bool throwIfNotFound = true) where TTemplate : class
        {
            return FindTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), throwIfNotFound);
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
