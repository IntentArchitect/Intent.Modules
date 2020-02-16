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
    public abstract class IntentProjectItemTemplateBase<TModel> : IntentTemplateBase, ITemplate, ITemplateWithModel, IProjectItemTemplate, IConfigurableTemplate, IHasTemplateDependencies, ITemplatePostConfigurationHook, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        protected readonly ICollection<ITemplateDependency> DetectedDependencies = new List<ITemplateDependency>();

        public IntentProjectItemTemplateBase(string templateId, IProject project, TModel model)
        {
            Project = project;
            Id = templateId;
            Model = model;
            Context = new TemplateContext(this);
        }

        public string Id { get; }
        public TModel Model { get; }
        public IProject Project { get; }
        public ITemplateContext Context { get; }
        public IFileMetadata FileMetadata { get; private set; }

        object ITemplateWithModel.Model
        {
            get
            {
                return this.Model;
            }
        }

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
            return GetTemplateClassName(TemplateDependency.OnModel(templateId, model));
        }

        public string GetTemplateClassName(string templateId, string modelId)
        {
            return GetTemplateClassName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId));
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
            return GetTemplateClassName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId), throwIfNotFound);
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
            return FindTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId));
        }

        public TTemplate FindTemplate<TTemplate>(ITemplateDependency dependency, bool throwIfNotFound) where TTemplate : class
        {
            if (!_onCreatedHasHappened)
            {
                throw new Exception($"${nameof(GetTemplateClassName)} cannot be called during template instantiation.");
            }

            var template = Project.Application.FindTemplateInstance<TTemplate>(dependency);
            if (template != null)
            {
                DetectedDependencies.Add(dependency);
                return template;
            }

            if (throwIfNotFound)
            {
                throw new Exception($"Could not find template with Id: {dependency.TemplateIdOrName} for model {Model.ToString()}");
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
            return FindTemplate<TTemplate>(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId), throwIfNotFound);
        }

        public override string ToString()
        {
            return $"{Id}#{Model?.ToString()}";
        }
    }

    public class TemplateContext : ITemplateContext
    {
        private object _defaultModelContext;
        private Dictionary<string, object> _prefixLookup;

        public TemplateContext(object defaultModelContext)
        {
            _defaultModelContext = defaultModelContext;
        }

        public TemplateContext() : this(null)
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

        public object GetRootContext(string propertyName, out bool isDefault)
        {
            if (_prefixLookup != null && _prefixLookup.ContainsKey(propertyName))
            {
                isDefault = false;
                return _prefixLookup[propertyName];
            }
            else
            {
                isDefault = true;
                return _defaultModelContext;
            }
        }
    }

    public abstract class IntentProjectItemTemplateBase : IntentProjectItemTemplateBase<object>
    {
        public IntentProjectItemTemplateBase(string templateId, IProject project) : base(templateId, project, null)
        {

        }

    }
}
