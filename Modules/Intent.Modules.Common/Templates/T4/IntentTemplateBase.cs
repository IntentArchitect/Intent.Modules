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

        public override string GetCorrelationId()
        {
            return $"{Id}{(Model as IMetadataModel)?.Id ?? ""}";
        }

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

        /// <summary>
        /// Unique identifier for this template. Must be unique in the application in which this template is installed.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Software Factory Execution context. Gives access to application-wide services.
        /// </summary>
        public ISoftwareFactoryExecutionContext ExecutionContext { get; }

        /// <summary>
        /// The OutputTarget of this template. This is determined by a designer with Output Targeting capabilities (e.g. Visual Studio, Folder Structure, etc.)
        /// </summary>
        public IOutputTarget OutputTarget { get; }
        public ITemplateBindingContext BindingContext { get; }
        public IFileMetadata FileMetadata { get; private set; }

        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            FileMetadata = fileMetadata;
            FileMetadata.CustomMetadata.TryAdd("CorrelationId", GetCorrelationId());
        }

        public abstract ITemplateFileConfig GetTemplateFileConfig();

        public virtual string RunTemplate()
        {
            // NOTE: If this method is run multiple times for a template instance, the output is duplicated. Perhaps put in a check here?
            return TransformText();
        }

        /// <summary>
        /// Used to identify template outputs between software factory executions.
        /// </summary>
        /// <returns></returns>
        public virtual string GetCorrelationId()
        {
            return Id;
        }

        public IFileMetadata GetMetadata()
        {
            return FileMetadata;
        }

        /// <summary>
        /// Adds the Template with <paramref name="templateId"/> as a dependency of this template.
        /// </summary>
        /// <param name="templateId"></param>
        public void AddTemplateDependency(string templateId)
        {
            AddTemplateDependency(TemplateDependency.OnTemplate(templateId));
        }

        /// <summary>
        /// Adds the Template with <paramref name="templateId"/> and <paramref name="model"/> as a dependency of this template.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="model">The metadata modle instance that the Template must be bound to.</param>
        public void AddTemplateDependency(string templateId, IMetadataModel model)
        {
            AddTemplateDependency(TemplateDependency.OnModel(templateId, model));
        }

        /// <summary>
        /// Adds the <see cref="ITemplateDependency"/> <paramref name="templateDependency"/> as a dependency of this template.
        /// </summary>
        /// <param name="templateDependency"></param>
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

        public virtual void OnCreated()
        {
        }

        /// <summary>
        /// Executed before the Template's <see cref="RunTemplate"/> runs.
        /// </summary>
        public virtual void BeforeTemplateExecution()
        {
        }


        public void SetDefaultTypeCollectionFormat(string collectionFormat)
        {
            if (!HasTypeResolver())
            {
                throw new Exception($"A {nameof(ITypeResolver)} has not been set for this Template at this time. Set {nameof(Types)} before calling this operation.");
            }
            Types.SetDefaultCollectionFormatter(new CollectionFormatter(collectionFormat));
        }

        public void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter)
        {
            if (!HasTypeResolver())
            {
                throw new Exception($"A {nameof(ITypeResolver)} has not been set for this Template at this time. Set {nameof(Types)} before calling this operation.");
            }
            Types.SetDefaultCollectionFormatter(collectionFormatter);
        }

        /// <summary>
        /// Adds the <see cref="ITypeSource"/> <paramref name="typeSource"/> as a source to find fully qualified types when using the <see cref="GetTypeName(ITypeReference)"/> method.
        /// </summary>
        /// <param name="typeSource"></param>
        public void AddTypeSource(ITypeSource typeSource)
        {
            Types.AddTypeSource(typeSource);
        }

        /// <summary>
        /// Adds a Template source that will be search when resolving <see cref="ITypeReference"/> types through the <see cref="IntentTemplateBase.GetTypeName(ITypeReference)"/>
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="collectionFormat">Sets the collection type to be used if a type is found.</param>
        public ClassTypeSource AddTypeSource(string templateId, string collectionFormat)
        {
            return AddTypeSource(templateId)
                .WithCollectionFormatter(new CollectionFormatter(collectionFormat));
        }

        /// <summary>
        /// Adds a Template source that will be search when resolving <see cref="ITypeReference"/> types through the <see cref="IntentTemplateBase.GetTypeName(ITypeReference)"/>
        /// </summary>
        /// <param name="templateId"></param>
        public ClassTypeSource AddTypeSource(string templateId)
        {
            var typeSource = ClassTypeSource.Create(ExecutionContext, templateId);
            AddTypeSource(typeSource);
            return typeSource;
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

        /// <summary>
        /// Gets the <see cref="IResolvedTypeInfo"/> for the resolved <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string.
        /// </summary>
        /// <param name="templateDependency"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            var name = GetTemplate<IClassProvider>(templateDependency, options)?.FullTypeName();
            return name != null ? NormalizeTypeName(name) : null;
        }

        /// <summary>
        /// Resolves the type name of the <paramref name="template"/> as a string.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string GetTypeName(ITemplate template, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnTemplate(template), options);
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// Will throw an exception if more than one template instance exists.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string GetTypeName(string templateId, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnTemplate(templateId), options);
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// Will return null if the template instance cannot be found.
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public string TryGetTypeName(string templateId)
        {
            return GetTypeName(TemplateDependency.OnTemplate(templateId), new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="model"/>.
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string GetTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnModel(templateId, model), options);
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="model"/>.
        /// Will return null if a template instance cannot be found.
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <returns></returns>
        public string TryGetTypeName(string templateId, IMetadataModel model)
        {
            return GetTypeName(TemplateDependency.OnModel(templateId, model), new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
        }


        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="modelId"/>.
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string GetTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), options);
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="modelId"/>.
        /// Will return null if a template instance cannot be found.
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <returns></returns>
        public string TryGetTypeName(string templateId, string modelId)
        {
            return GetTypeName(TemplateDependency.OnModel<IMetadataModel>(templateId, x => x.Id == modelId, $"Model Id: {modelId}"), new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
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
            // TODO: Bring this back to prevent unexpected behaviour.
            //if (!_onCreatedHasHappened)
            //{
            //    throw new Exception($"{nameof(GetTypeName)} cannot be called during template instantiation.");
            //}

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
