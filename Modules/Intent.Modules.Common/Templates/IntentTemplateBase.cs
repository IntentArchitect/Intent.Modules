using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    /// <summary>
    /// A base class for templates which use both models and decorators.
    /// </summary>
    public abstract class IntentTemplateBase<TModel, TDecorator> : IntentTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        /// <summary>
        /// Constructor for <see cref="IntentTemplateBase{TModel,TDecorator}"/>.
        /// </summary>
        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <summary>
        /// Returns all decorators that have been added to this template.
        /// </summary>
        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        /// <summary>
        /// Adds a decorator to this template. This is called automatically by the Intent Architect software factory when a decorator is resolved for this template.
        /// </summary>
        public void AddDecorator(TDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        /// <summary>
        /// Aggregates decorator outputs for the property specified by <paramref name="propertyFunc"/>
        /// </summary>
        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc)
        {
            return GetDecorators().Aggregate(propertyFunc);
        }
    }

    /// <summary>
    /// A base class for templates which use models.
    /// </summary>
    public abstract class IntentTemplateBase<TModel> : IntentTemplateBase, ITemplateWithModel
    {
        /// <summary>
        /// Constructor for <see cref="IntentTemplateBase{TModel}"/>.
        /// </summary>
        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget)
        {
            Model = model;
        }

        /// <summary>
        /// Model
        /// </summary>
        public TModel Model { get; }

        object ITemplateWithModel.Model => Model;

        /// <inheritdoc />
        public override string GetCorrelationId()
        {
            if (Model is IMetadataModel model)
            {
                return $"{Id}#{model.Id}";
            }

            if (Model == null || Model is IEnumerable)
            {
                return Id;
            }

            return null;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Id} [{Model?.ToString()}]";
        }
    }

    /// <summary>
    /// Base class for templates.
    /// </summary>
    public abstract class IntentTemplateBase : T4TemplateBase, ITemplate, IConfigurableTemplate, IHasTemplateDependencies, ITemplatePostConfigurationHook, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        private readonly HashSet<ITemplate> _detectedDependencies = new HashSet<ITemplate>();

        /// <summary>
        /// Obsolete. Use <see cref="_detectedDependencies"/> instead. See <see cref="GetTemplateDependencies"/>
        /// for more information.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version_3_2_0)]
        protected readonly ICollection<ITemplateDependency> DetectedDependencies = new List<ITemplateDependency>();

        /// <summary>
        /// Constructor for <see cref="IntentTemplateBase"/>.
        /// </summary>
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

        /// <inheritdoc />
        public ITemplateBindingContext BindingContext { get; }

        /// <summary>
        /// Metadata of the template.
        /// </summary>
        public IFileMetadata FileMetadata { get; private set; }

        /// <inheritdoc />
        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            FileMetadata = fileMetadata;
            FileMetadata.CustomMetadata.TryAdd("CorrelationId", GetCorrelationId());
        }

        /// <inheritdoc />
        public abstract ITemplateFileConfig GetTemplateFileConfig();

        /// <summary>
        /// Override this method to control whether the template runs and the creates the output file.
        /// </summary>
        public virtual bool CanRunTemplate()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual string RunTemplate()
        {
            // NOTE: If this method is run multiple times for a template instance, the output is duplicated. Perhaps put in a check here?
            return TransformText();
        }

        /// <summary>
        /// Used to identify template outputs between software factory executions.
        /// </summary>
        public virtual string GetCorrelationId()
        {
            return Id;
        }

        /// <inheritdoc />
        public IFileMetadata GetMetadata()
        {
            return FileMetadata;
        }

        /// <summary>
        /// Adds the Template with <paramref name="templateId"/> as a dependency of this template.
        /// </summary>
        public void AddTemplateDependency(string templateId)
        {
            AddTemplateDependency(ExecutionContext.FindTemplateInstance(templateId));
        }

        /// <summary>
        /// Adds the Template with <paramref name="templateId"/> and <paramref name="model"/> as a dependency of this template.
        /// </summary>
        /// <param name="templateId">The id of the template to be dependent upon.</param>
        /// <param name="model">The metadata module instance that the Template must be bound to.</param>
        public void AddTemplateDependency(string templateId, IMetadataModel model)
        {
            AddTemplateDependency(ExecutionContext.FindTemplateInstance(templateId, model.Id));
        }

        /// <summary>
        /// Adds the <see cref="ITemplateDependency"/> <paramref name="templateDependency"/> as a dependency of this template.
        /// </summary>
        public void AddTemplateDependency(ITemplateDependency templateDependency)
        {
            AddTemplateDependency(ExecutionContext.FindTemplateInstance(templateDependency));
        }

        private void AddTemplateDependency(ITemplate template)
        {
#pragma warning disable CS0618
            DetectedDependencies.Add(TemplateDependency.OnTemplate(template));
#pragma warning restore CS0618
            _detectedDependencies.Add(template);
        }

        /// <summary>
        /// For 3.2.0, change this to return <see cref="IEnumerable{ITemplate}"/> and
        /// use <see cref="_detectedDependencies"/> instead of <see cref="DetectedDependencies"/>,
        /// so the method becomes:
        /// <code>
        /// <![CDATA[
        /// public IEnumerable<ITemplate> GetTemplateDependencies()
        /// {
        ///     if (!HasTypeResolver())
        ///     {
        ///         return _detectedDependencies;
        ///     }
        /// 
        ///     return Types.GetTemplateDependencies().Concat(_detectedDependencies);
        /// }
        /// ]]>
        /// </code>
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version_3_3_0)]
        [FixFor_3_2_0]
        public virtual IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            if (!HasTypeResolver())
            {
                return DetectedDependencies;
            }

            return Types.GetTemplateDependencies().Concat(DetectedDependencies);
        }

        /// <summary>
        /// Called after all templates have been configured.
        /// </summary>
        public virtual void OnConfigured()
        {
        }

        /// <summary>
        /// Called after all templates have been created.
        /// </summary>
        public virtual void OnCreated()
        {
        }

        /// <summary>
        /// Executed before the Template's <see cref="RunTemplate"/> runs.
        /// </summary>
        public virtual void BeforeTemplateExecution()
        {
        }

        /// <summary>
        /// Sets the default collection format to be applied to types that are resolved using the <see cref="GetTypeName(ITypeReference)"/> method.
        /// </summary>
        public void SetDefaultTypeCollectionFormat(string collectionFormat)
        {
            if (!HasTypeResolver())
            {
                throw new Exception($"A {nameof(ITypeResolver)} has not been set for this Template at this time. Set {nameof(Types)} before calling this operation.");
            }
            Types.SetDefaultCollectionFormatter(new CollectionFormatter(collectionFormat));
        }

        /// <summary>
        /// Sets the default collection formatter to be applied to types that are resolved using the <see cref="GetTypeName(ITypeReference)"/> method.
        /// </summary>
        public void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter)
        {
            if (!HasTypeResolver())
            {
                throw new Exception($"A {nameof(ITypeResolver)} has not been set for this Template at this time. Set {nameof(Types)} before calling this operation.");
            }
            Types.SetDefaultCollectionFormatter(collectionFormatter);
        }

        /// <summary>
        /// Adds the <paramref name="typeSource"/> as a source to find fully qualified types when using the <see cref="GetTypeName(ITypeReference)"/> method.
        /// If found, the Template will be added as a dependency.
        /// </summary>
        public void AddTypeSource(ITypeSource typeSource)
        {
            Types.AddTypeSource(typeSource);
        }

        /// <summary>
        /// Adds a Template source (template instances) that will be search when resolving <see cref="ITypeReference"/> types through the <see cref="IntentTemplateBase.GetTypeName(ITypeReference)"/>.
        /// If found, the Template will be added as a dependency.
        /// Set the desired <see cref="CollectionFormatter"/> for when the type is resolved from this type-source by calling .WithCollectionFormatter(...).
        /// </summary>
        /// <param name="templateId">The identifier of the template instances to be searched when calling <see cref="IntentTemplateBase.GetTypeName(ITypeReference)"/></param>
        /// <param name="collectionFormat">Sets the collection format to be applied if a type is found.</param>
        public ClassTypeSource AddTypeSource(string templateId, string collectionFormat)
        {
            return AddTypeSource(templateId)
                .WithCollectionFormatter(new CollectionFormatter(collectionFormat));
        }

        /// <summary>
        /// Adds a Template source that will be searched when resolving <see cref="ITypeReference"/> types through the <see cref="IntentTemplateBase.GetTypeName(ITypeReference)"/>.
        /// If found, the Template will be added as a dependency.
        /// Set the desired <see cref="CollectionFormatter"/> for when the type is resolved from this type-source by calling .WithCollectionFormatter(...).
        /// </summary>
        /// <param name="templateId">The identifier of the template instances to be searched when calling <see cref="GetTypeName(ITypeReference)"/>.</param>
        /// <returns>Returns the <see cref="ClassTypeSource"/> for use as a fluent api.</returns>
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
        public virtual string NormalizeTypeName(string name)
        {
            return name;
        }

        /// <summary>
        /// Resolves the <see cref="IResolvedTypeInfo"/> for the resolved <paramref name="typeReference"/>.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        public virtual IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference)
        {
            return Types.Get(typeReference);
        }

        /// <summary>
        /// Resolves and <see cref="NormalizeTypeName">normalizes</see> the type name for the <paramref name="typeReference"/> parameter.
        /// Any added <see cref="ITypeSource"/> added by <see cref="AddTypeSource(ITypeSource)"/> will be searched to resolve the type name.
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/> is true.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true</param>
        public virtual string GetTypeName(ITypeReference typeReference, string collectionFormat)
        {
            return NormalizeTypeName(Types.Get(typeReference, collectionFormat).Name);
        }

        /// <summary>
        /// <para>
        /// Resolves and <see cref="NormalizeTypeName">normalizes</see> the type name for the <paramref name="typeReference"/> parameter.
        /// Any added <see cref="ITypeSource"/> added by <see cref="AddTypeSource(ITypeSource)"/> will be searched to resolve the type name.
        /// </para>
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the type name.</param>
        public virtual string GetTypeName(ITypeReference typeReference)
        {
            return NormalizeTypeName(Types.Get(typeReference).Name);
        }

        /// <summary>
        /// Resolves and <see cref="NormalizeTypeName">normalizes</see> the type name for the <paramref name="hasTypeReference."/> parameter.
        /// Any added <see cref="ITypeSource"/> added by <see cref="AddTypeSource(ITypeSource)"/> will be searched to resolve the type name.
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/> is true.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="hasTypeReference">The <see cref="IHasTypeReference"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true</param>
        public virtual string GetTypeName(IHasTypeReference hasTypeReference, string collectionFormat)
        {
            return GetTypeName(hasTypeReference.TypeReference, collectionFormat);
        }

        /// <summary>
        /// Resolves and <see cref="NormalizeTypeName">normalizes</see> the type name for the <paramref name="hasTypeReference"/> parameter.
        /// Any added <see cref="ITypeSource"/> added by <see cref="AddTypeSource(ITypeSource)"/> will be searched to resolve the type name.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="hasTypeReference">The <see cref="IHasTypeReference"/> for which to get the type name.</param>
        public virtual string GetTypeName(IHasTypeReference hasTypeReference)
        {
            return GetTypeName(hasTypeReference.TypeReference);
        }

        /// <summary>
        /// Resolves and <see cref="NormalizeTypeName">normalizes</see> the type name for the <paramref name="element"/> parameter.
        /// Any added <see cref="ITypeSource"/> added by <see cref="AddTypeSource(ITypeSource)"/> will be searched to resolve the type name.
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/> is true.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="element">The <see cref="IElement"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true</param>
        public virtual string GetTypeName(IElement element, string collectionFormat)
        {
            return NormalizeTypeName(Types.Get(element, collectionFormat).Name);
        }

        ///// <summary>
        ///// Resolves and <see cref="NormalizeTypeName">normalizes</see> the type name for the <paramref name="element"/> parameter.
        ///// Any added <see cref="ITypeSource"/> added by <see cref="AddTypeSource(ITypeSource)"/> will be searched to resolve the type name.
        ///// <para>
        ///// See the
        ///// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        ///// GetTypeName article</seealso> for more information.
        ///// </para>
        ///// </summary>
        //public virtual string GetTypeName(ICanBeReferencedType element)
        //{
        //    return NormalizeTypeName(Types.Get(element).Name);
        //}

        #endregion

        #region GetTypeName for Template

        /// <summary>
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        public virtual string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateDependency, options));
        }

        /// <summary>
        /// Resolves the type name of the <paramref name="template"/> as a string.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        public string GetTypeName(ITemplate template, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(GetTemplate<IClassProvider>(template, options));
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// Will throw an exception if more than one template instance exists.
        /// </summary>
        public string GetTypeName(string templateId, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateId, options));
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// Will return null if the template instance cannot be found.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        public string TryGetTypeName(string templateId)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateId, TemplateDiscoveryOptions.DoNotThrow));
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="model"/>.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateId, model, options));
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="model"/>.
        /// Will return null if a template instance cannot be found.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        public string TryGetTypeName(string templateId, IMetadataModel model)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateId, model, TemplateDiscoveryOptions.DoNotThrow));
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="modelId"/>.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateId, modelId, options));
        }

        /// <summary>
        /// Resolves the type name of the Template with <paramref name="templateId"/> as a string.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="modelId"/>.
        /// Will return null if a template instance cannot be found.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// GetTypeName article</seealso> for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        public string TryGetTypeName(string templateId, string modelId)
        {
            return GetTypeName(GetTemplate<IClassProvider>(templateId, modelId, TemplateDiscoveryOptions.DoNotThrow));
        }

        private string GetTypeName(IClassProvider classProvider)
        {
            var name = classProvider.FullTypeName();
            if (name == null)
            {
                return null;
            }

            return NormalizeTypeName(name);
        }

        #endregion

        #region GetTemplate

        /// <summary>
        /// Retrieve an instance of an <see cref="ITemplate"/>.
        /// </summary>
        public TTemplate GetTemplate<TTemplate>(string templateId, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => ExecutionContext.FindTemplateInstance<TTemplate>(templateId),
                getDependencyDescriptionForException: () => $"TemplateId = {templateId}",
                options: options);
        }

        /// <inheritdoc cref="GetTemplate{TTemplate}(string,TemplateDiscoveryOptions)"/>
        public TTemplate GetTemplate<TTemplate>(string templateId, string modelId, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => ExecutionContext.FindTemplateInstance<TTemplate>(templateId, modelId),
                getDependencyDescriptionForException: () => $"TemplateId = {templateId}, ModelId = {modelId}",
                options: options);

        }

        /// <inheritdoc cref="GetTemplate{TTemplate}(string,TemplateDiscoveryOptions)"/>
        public TTemplate GetTemplate<TTemplate>(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => ExecutionContext.FindTemplateInstance<TTemplate>(templateId, model.Id),
                getDependencyDescriptionForException: () => $"TemplateId = {templateId}, model.Id = {model.Id}",
                options: options);
        }

        /// <inheritdoc cref="GetTemplate{TTemplate}(string,TemplateDiscoveryOptions)"/>
        public TTemplate GetTemplate<TTemplate>(ITemplateDependency dependency, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => (TTemplate)ExecutionContext.FindTemplateInstance<ITemplate>(dependency),
                getDependencyDescriptionForException: dependency.ToString,
                options: options);
        }

        private TTemplate GetTemplate<TTemplate>(ITemplate template, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => template as TTemplate,
                getDependencyDescriptionForException: () => $"{template} as is not a {typeof(TTemplate).Name}",
                options: options);
        }

        /// <remarks>
        /// For 3.2.0 we want to add a generic type parameter constraint where <typeparamref name="TTemplate"/>
        /// must of type <see cref="ITemplate"/>.<br/>
        /// <br/>
        /// By implication, all the public overloads of this will need the same constraint applied.<br/>
        /// <br/>
        /// A blocker for this is that some code in other modules is as follows:
        /// <code>
        /// <![CDATA[
        /// GetTemplate<IModuleBuilderTemplate>
        /// ]]>
        /// </code>
        /// Which means that we will probably need to make IModuleBuilderTemplate derive from
        /// <see cref="ITemplate"/>.
        /// </remarks>
        [FixFor_3_2_0]
        private TTemplate GetTemplate<TTemplate>(
            Func<TTemplate> getTemplate,
            Func<string> getDependencyDescriptionForException,
            TemplateDiscoveryOptions options = null)
        {
            if (options == null)
            {
                options = new TemplateDiscoveryOptions();
            }

            var template = getTemplate();
            if (template == null && options.ThrowIfNotFound)
            {
                throw new Exception($"Could not find template from dependency: {getDependencyDescriptionForException()}");
            }

            if (options.TrackDependency && template != null)
            {
                AddTemplateDependency((ITemplate)template);
            }

            return template;
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Id}";
        }
    }

    /// <summary>
    /// Template discovery options.
    /// </summary>
    public class TemplateDiscoveryOptions
    {
        /// <summary>
        /// Throw an exception if the template is not found. Defaults to <see langword="true"/>.
        /// </summary>
        public bool ThrowIfNotFound { get; set; } = true;

        /// <summary>
        /// Whether or not to automatically track the template as a dependency. Defaults to <see langword="true"/>.
        /// </summary>
        public bool TrackDependency { get; set; } = true;

        /// <summary>
        /// An instance of <see cref="TemplateDiscoveryOptions"/> where <see cref="ThrowIfNotFound"/> is set to false.
        /// </summary>
        public static TemplateDiscoveryOptions DoNotThrow { get; } = new TemplateDiscoveryOptions { ThrowIfNotFound = false };
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
