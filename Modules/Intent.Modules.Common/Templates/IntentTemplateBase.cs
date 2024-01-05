using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public abstract class IntentTemplateBase<TModel, TDecorator> : IntentTemplateBase<TModel>,
        IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        /// <summary>
        /// Constructor for <see cref="IntentTemplateBase{TModel,TDecorator}"/>.
        /// </summary>
        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId,
            outputTarget, model)
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
    public abstract class IntentTemplateBase<TModel> : IntentTemplateBase, IIntentTemplate<TModel>, ITemplateWithModel
    {
        /// <summary>
        /// Constructor for <see cref="IntentTemplateBase{TModel}"/>.
        /// </summary>
        protected IntentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId,
            outputTarget)
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
    public abstract class IntentTemplateBase : T4TemplateBase, IIntentTemplate, ITemplate, IConfigurableTemplate,
        IHasTemplateDependencies, ITemplatePostConfigurationHook, ITemplatePostCreationHook, 
        IAfterTemplateRegistrationExecutionHook,
        ITemplateBeforeExecutionHook
    {
        private readonly Lazy<(bool Result, string Path)> _tryGetExistingFilePathCache;
        private readonly Lazy<(bool Result, string Content)> _tryGetExistingFileContentCache;

        /// <summary>
        /// Returns the known template dependencies added for this template.
        /// </summary>
        [FixFor_Version4("Change name to KnownTemplateDependencies. Consider making private.")]
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
            _tryGetExistingFilePathCache = new Lazy<(bool, string)>(TryGetExistingFilePathInternal);
            _tryGetExistingFileContentCache = new Lazy<(bool, string)>(TryGetExistingFileContentInternal);
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

        /// <summary>
        /// Creates an instance of <see cref="ICollectionFormatter"/>. Override this method to return
        /// a different implementation of <see cref="ICollectionFormatter"/>.
        /// </summary>
        protected virtual ICollectionFormatter CreateCollectionFormatter(string collectionFormat)
        {
            return CollectionFormatter.Create(collectionFormat);
        }

        /// <inheritdoc />
        public abstract ITemplateFileConfig GetTemplateFileConfig();

        /// <summary>
        /// Returns the file path of the existing file for this template, if it exists. If it doesn't exist, or can't be found, will return null.
        /// </summary>
        public virtual string GetExistingFilePath() => _tryGetExistingFilePathCache.Value.Path;

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
            // NOTE: If this method is run multiple times for a template instance, the output is duplicated (StringBuilder under the hood). Perhaps put in a check here?
            var text = TransformText();
            // Normalize line endings to that of the current platform
            text = NewlinePattern.Replace(text, Environment.NewLine);
            return text;
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

        #region AddTemplateDependency

        private void AddTemplateDependency(ITemplate template)
        {
            DetectedDependencies.Add(TemplateDependency.OnTemplate(template));
        }

        /// <summary>
        /// Adds the <see cref="ITemplateDependency"/> <paramref name="templateDependency"/> as a dependency of this template.
        /// </summary>
        public void AddTemplateDependency(ITemplateDependency templateDependency)
        {
            // Can't use this because this method is often called from constructors, and the FindTemplateInstance has the chance of not
            // finding the template and then throwing an error. If no bugs found in Intent.Common 3.1.12 then this code can be removed.
            //AddTemplateDependency(ExecutionContext.FindTemplateInstance(templateDependency));
            DetectedDependencies.Add(templateDependency);
        }

        /// <summary>
        /// Adds the Template with <paramref name="templateId"/> as a dependency of this template.
        /// </summary>
        public void AddTemplateDependency(string templateId)
        {
            // Can't use this because this method is often called from constructors, and the FindTemplateInstance has the chance of not
            // finding the template and then throwing an error. If no bugs found in Intent.Common 3.1.12 then this code can be removed.
            //AddTemplateDependency(ExecutionContext.FindTemplateInstance(templateId));
            DetectedDependencies.Add(TemplateDependency.OnTemplate(templateId));
        }

        /// <summary>
        /// Adds the Template with <paramref name="templateId"/> and <paramref name="model"/> as a dependency of this template.
        /// </summary>
        /// <param name="templateId">The id of the template to be dependent upon.</param>
        /// <param name="model">The metadata module instance that the Template must be bound to.</param>
        public void AddTemplateDependency(string templateId, IMetadataModel model)
        {
            // Can't use this because this method is often called from constructors, and the FindTemplateInstance has the chance of not
            // finding the template and then throwing an error. If no bugs found in Intent.Common 3.1.12 then this code can be removed.
            //AddTemplateDependency(ExecutionContext.FindTemplateInstance(templateId, model.Id));
            DetectedDependencies.Add(TemplateDependency.OnModel(templateId, model));
        }

        #endregion

        /// <summary>
        /// Returns all template dependencies detected for this template.
        /// </summary>
        /// <returns></returns>
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
        /// Called after the current Template has been created and registered.
        /// </summary>
        public virtual void OnCreated()
        {
        }

        /// <summary>
        /// Executed after all Templates have been registered.
        /// </summary>
        public virtual void AfterTemplateRegistration()
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
                throw new Exception(
                    $"A {nameof(ITypeResolver)} has not been set for this Template at this time. Set {nameof(Types)} before calling this operation.");
            }

            Types.SetDefaultCollectionFormatter(CreateCollectionFormatter(collectionFormat));
        }

        /// <summary>
        /// Sets the default collection formatter to be applied to types that are resolved using the <see cref="GetTypeName(ITypeReference)"/> method.
        /// </summary>
        public void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter)
        {
            if (!HasTypeResolver())
            {
                throw new Exception(
                    $"A {nameof(ITypeResolver)} has not been set for this Template at this time. Set {nameof(Types)} before calling this operation.");
            }

            Types.SetDefaultCollectionFormatter(collectionFormatter);
        }

        /// <summary>
        /// Adds the <paramref name="typeSource"/> as a source to find fully qualified types when
        /// using the <see cref="GetTypeName(ITypeReference)"/> method. If found, the Template will
        /// be added as a dependency.
        /// </summary>
        public void AddTypeSource(ITypeSource typeSource)
        {
            Types.AddTypeSource(typeSource);
        }

        /// <summary>
        /// Adds a Template source that will be searched when resolving <see cref="ITypeReference"/>
        /// types through the <see cref="GetTypeName(ITypeReference)"/>. If found, the Template will
        /// be added as a dependency. Set the desired <see cref="CollectionFormatter"/> for when the
        /// type is resolved from this type-source by calling <see cref="ClassTypeSource.WithCollectionFormatter"/>.
        /// </summary>
        /// <param name="templateId">The identifier of the template instances to be searched when calling <see cref="GetTypeName(ITypeReference)"/>.</param>
        /// <returns>Returns the <see cref="ClassTypeSource"/> for use as a fluent api.</returns>
        public ClassTypeSource AddTypeSource(string templateId)
        {
            return AddTypeSource(templateId, null);
        }

        /// <summary>
        /// Adds a Template source that will be searched when resolving <see cref="ITypeReference"/>
        /// types through the <see cref="GetTypeName(ITypeReference)"/>. If found, the Template will
        /// be added as a dependency. Set the desired <see cref="CollectionFormatter"/> for when the
        /// type is resolved from this type-source by calling <see cref="ClassTypeSource.WithCollectionFormatter"/>.
        /// </summary>
        /// <param name="templateId">The identifier of the template instances to be searched when calling <see cref="GetTypeName(ITypeReference)"/></param>
        /// <param name="collectionFormat">Sets the collection format to be applied if a type is found.</param>
        [FixFor_Version4("Make \"collectionFormat\" have a default value of null and delete conflicting overload")]
        public virtual ClassTypeSource AddTypeSource(string templateId, string collectionFormat)
        {
            var typeSource = ClassTypeSource.Create(ExecutionContext, templateId, CreateCollectionFormatter)
                .WithNullFormatter(Types.DefaultNullableFormatter);

            if (!string.IsNullOrWhiteSpace(collectionFormat))
            {
                typeSource = typeSource.WithCollectionFormatter(CreateCollectionFormatter(collectionFormat));
            }

            Types.AddTypeSource(typeSource);

            return typeSource;
        }

        #region NormalizeTypeName

        /// <summary>
        /// Normalizes a type name in a language specific manner. The <see cref="NormalizeTypeName">
        /// implementation</see> on <see cref="IntentTemplateBase{TModel}"/>
        /// not alter the provided <paramref name="name"/>, <see langword="override"/> this method
        /// to apply any desired normalization.
        /// <param name="name">The type name to normalize.</param>
        /// </summary>
        public virtual string NormalizeTypeName(string name)
        {
            return name;
        }

        #endregion

        /// <summary>
        /// Signifies that this template fulfills the specified <paramref name="role"/> in the architecture. Other templates can search for templates that
        /// fulfill roles and thereby find them in a decoupled way. This method is deprecated since registering templates against their
        /// roles will be done automatically by the Software Factory execution in future releases.
        /// </summary>
        public void FulfillsRole(string role)
        {
            ExecutionContext.RegisterTemplateInRole(role, this);
        }

        #region GetTypeInfo

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="classProvider"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        protected virtual IResolvedTypeInfo GetTypeInfo(IClassProvider classProvider)
        {
            if (!HasTypeResolver())
            {
                return ResolvedTypeInfo.Create(
                    name: classProvider.FullTypeName(),
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    template: classProvider,
                    nullableFormatter: null,
                    collectionFormatter: null);
            }

            return Types.Get(classProvider);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="element"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the <see cref="IResolvedTypeInfo"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="element">The <see cref="IElement"/> for which to get the <see cref="IResolvedTypeInfo"/>.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(IElement element)
        {
            return Types.Get(element);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetTypeInfo(IElement)"/> instead.
        /// </summary>
        /// <remarks>
        /// Even before this method was marked as obsolete, the <paramref name="collectionFormat"/>
        /// value actually had no effect.
        /// </remarks>
        [Obsolete(WillBeRemovedIn.Version4)]
        protected virtual IResolvedTypeInfo GetTypeInfo(IElement element, string collectionFormat)
        {
            return GetTypeInfo(element);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="hasTypeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the <see cref="IResolvedTypeInfo"/>.
        /// 
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/>
        /// is true.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="hasTypeReference">The <see cref="IHasTypeReference"/> for which to get the <see cref="IResolvedTypeInfo"/>.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(IHasTypeReference hasTypeReference, string collectionFormat = null)
        {
            return GetTypeInfo(hasTypeReference.TypeReference, collectionFormat);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="template"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/> for which to get the <see cref="IResolvedTypeInfo"/>.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(ITemplate template, TemplateDiscoveryOptions options = null)
        {
            var classProvider = GetTemplate<IClassProvider>(template, options);
            if (classProvider == null)
            {
                return null;
            }

            return GetTypeInfo(classProvider);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="templateDependency"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateDependency">The <see cref="ITemplateDependency"/> for which to get the <see cref="IResolvedTypeInfo"/>.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(ITemplateDependency templateDependency,
            TemplateDiscoveryOptions options = null)
        {
            var classProvider = GetTemplate<IClassProvider>(templateDependency, options);
            if (classProvider == null)
            {
                return null;
            }

            return GetTypeInfo(classProvider);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="typeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the <see cref="IResolvedTypeInfo"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the <see cref="IResolvedTypeInfo"/>.</param>
        [FixFor_Version4("Remove this method as it is hiding the GetTypeInfo(ITypeReference typeReference, string collectionFormat = null) overload.")]
        public virtual IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference)
        {
            return GetTypeInfo(typeReference, null);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="typeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the <see cref="IResolvedTypeInfo"/>.
        /// 
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/>
        /// is true.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the <see cref="IResolvedTypeInfo"/>.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true.</param>
        [FixFor_Version4("Remove the GetTypeInfo(ITypeReference typeReference) overload and then remove the ReSharper disable once MethodOverloadWithOptionalParameter below.")]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public virtual IResolvedTypeInfo GetTypeInfo(ITypeReference typeReference, string collectionFormat = null)
        {
            return Types.Get(typeReference, collectionFormat);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="templateId"/>
        /// and <paramref name="model"/> parameters.
        /// 
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the provided <paramref name="model"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(string templateId, IMetadataModel model,
            TemplateDiscoveryOptions options = null)
        {
            var classProvider = GetTemplate<IClassProvider>(templateId, model, options);
            if (classProvider == null)
            {
                return null;
            }

            return GetTypeInfo(classProvider);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="templateId"/>
        /// and <paramref name="modelId"/> parameters.
        /// 
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the provided <paramref name="modelId"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(string templateId, string modelId,
            TemplateDiscoveryOptions options = null)
        {
            var classProvider = GetTemplate<IClassProvider>(templateId, modelId, options);
            if (classProvider == null)
            {
                return null;
            }
            
            return GetTypeInfo(classProvider);
        }

        /// <summary>
        /// Resolves an <see cref="IResolvedTypeInfo"/> for the provided <paramref name="templateId"/>
        /// parameter.
        /// 
        /// This overload assumes that the Template only has a single instance and will throw an
        /// exception if more than one is found.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        protected virtual IResolvedTypeInfo GetTypeInfo(string templateId, TemplateDiscoveryOptions options = null)
        {
            var classProvider = GetTemplate<IClassProvider>(templateId, options);
            if (classProvider == null)
            {
                return null;
            }
            
            return GetTypeInfo(classProvider);
        }

        #endregion

        #region GetTypeName

        /// <summary>
        /// Resolves the type name for the provided <paramref name="classProvider"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        protected virtual string GetTypeName(IClassProvider classProvider)
        {
            var resolvedTypeInfo = GetTypeInfo(classProvider);

            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="element"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the type name.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="element">The <see cref="IElement"/> for which to get the type name.</param>
        public virtual string GetTypeName(IElement element)
        {
            var resolvedTypeInfo = GetTypeInfo(element);

            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetTypeName(IElement)"/> instead.
        /// </summary>
        /// <remarks>
        /// Even before this method was marked as obsolete, the <paramref name="collectionFormat"/>
        /// value actually had no effect.
        /// </remarks>
        [Obsolete(WillBeRemovedIn.Version4)]
        public virtual string GetTypeName(IElement element, string collectionFormat)
        {
            return GetTypeName(element);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="hasTypeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the type name.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="hasTypeReference">The <see cref="IHasTypeReference"/> for which to get the type name.</param>
        [FixFor_Version4("Remove this method as it is hiding the GetTypeName(IHasTypeReference hasTypeReference, string collectionFormat = null) overload.")]
        public virtual string GetTypeName(IHasTypeReference hasTypeReference)
        {
            var resolvedTypeInfo = GetTypeInfo(hasTypeReference.TypeReference);

            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="hasTypeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the type name.
        /// 
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/>
        /// is true.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="hasTypeReference">The <see cref="IHasTypeReference"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true.</param>
        [FixFor_Version4("Remove the GetTypeName(IHasTypeReference hasTypeReference) overload and then remove the ReSharper disable once MethodOverloadWithOptionalParameter below.")]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public virtual string GetTypeName(IHasTypeReference hasTypeReference, string collectionFormat = null)
        {
            var resolvedTypeInfo = GetTypeInfo(hasTypeReference.TypeReference, collectionFormat);

            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="template"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="template">The <see cref="ITemplate"/> for which to get the type name.</param>
        /// <param name="options"><see cref="TemplateDiscoveryOptions"/> to use.</param>
        public string GetTypeName(ITemplate template, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(template, options);
            if (resolvedTypeInfo == null)
            {
                return null;
            }
            
            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="templateDependency"/>
        /// parameter.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateDependency">The <see cref="ITemplateDependency"/> for which to get the type name.</param>
        /// <param name="options"><see cref="TemplateDiscoveryOptions"/> to use.</param>
        public virtual string GetTypeName(ITemplateDependency templateDependency,
            TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateDependency, options);
            if (resolvedTypeInfo == null)
            {
                return null;
            }
            
            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="typeReference"/>
        /// parameter.
        /// 
        /// Any source added by <see cref="AddTypeSource(ITypeSource)"/> or <see cref="AddTypeSource(string,string)"/>
        /// will be searched to resolve the <see cref="IResolvedTypeInfo"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the type name.</param>
        [FixFor_Version4("Remove this method as it is hiding the GetTypeName(ITypeReference typeReference, string collectionFormat = null) overload.")]
        public virtual string GetTypeName(ITypeReference typeReference)
        {
            var resolvedTypeInfo = GetTypeInfo(typeReference);

            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="typeReference"/>
        /// parameter.
        /// Any added <see cref="ITypeSource"/> by <see cref="AddTypeSource(ITypeSource)"/> will be
        /// searched to resolve the type name.
        /// Applies the <paramref name="collectionFormat"/> if the resolved type's <see cref="ITypeReference.IsCollection"/> is true.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="typeReference">The <see cref="ITypeReference"/> for which to get the type name.</param>
        /// <param name="collectionFormat">The collection format to be applied if the resolved type <see cref="ITypeReference.IsCollection"/> is true</param>
        [FixFor_Version4("Remove the GetTypeName(ITypeReference typeReference) overload and then remove the ReSharper disable once MethodOverloadWithOptionalParameter below.")]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public virtual string GetTypeName(ITypeReference typeReference, string collectionFormat = null)
        {
            var resolvedTypeInfo = GetTypeInfo(typeReference, collectionFormat);

            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="templateId"/>
        /// and <paramref name="model"/> parameters.
        /// 
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the provided <paramref name="model"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetTypeName(string templateId, IMetadataModel model, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, model, options);
            if (resolvedTypeInfo == null)
            {
                return null;
            }
            
            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="templateId"/>
        /// and <paramref name="modelId"/> parameters.
        /// 
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the provided <paramref name="modelId"/>.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetTypeName(string templateId, string modelId, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, modelId, options);
            if (resolvedTypeInfo == null)
            {
                return null;
            }
            
            return UseType(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolves the type name for the provided <paramref name="templateId"/>
        /// parameter.
        /// 
        /// This overload assumes that the Template only has a single instance and will throw an
        /// exception if more than one is found.
        /// 
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="options">Optional <see cref="TemplateDiscoveryOptions"/> to apply.</param>
        public string GetTypeName(string templateId, TemplateDiscoveryOptions options = null)
        {
            var resolvedTypeInfo = GetTypeInfo(templateId, options);
            if (resolvedTypeInfo == null)
            {
                return null;
            }
            
            return UseType(resolvedTypeInfo);
        }

        #endregion

        /// <summary>
        /// Tries to get existing file content of this template's output.
        /// </summary>
        /// <remarks>
        /// This method takes into account that the output path may have changed since the previous
        /// Software Factory execution.
        /// </remarks>
        /// <param name="content">The contents of the file if it exists.</param>
        /// <returns>whether there was an existing file for this template's output.</returns>
        public bool TryGetExistingFileContent(out string content)
        {
            (var result, content) = _tryGetExistingFileContentCache.Value;

            return result;
        }

        /// <summary>
        /// Not to be called directly, this is a delegate for the <see cref="Lazy{T}"/> instance
        /// for <see cref="_tryGetExistingFileContentCache"/>.
        /// </summary>
        private (bool result, string content) TryGetExistingFileContentInternal()
        {
            return TryGetExistingFilePath(out var path)
                ? (true, File.ReadAllText(path))
                : (false, default);
        }

        /// <summary>
        /// Not to be called directly, this is a delegate for the <see cref="Lazy{T}"/> instance
        /// for <see cref="_tryGetExistingFilePathCache"/>.
        /// </summary>
        /// <remarks>
        /// There is a significant performance impact even for <see cref="File.Exists"/> and
        /// caching them has been shown to make a highly significant improvement in software
        /// factory execution time.
        /// <para>
        /// It is intentional that if a file exists at the current output path then the file at the
        /// current output path is considered the "existing" file, regardless of whether the
        /// output path is different compared to the previous software execution.
        /// </para>
        /// <para>
        /// This is so that the following scenario works as expected:
        /// - Rename the file preemptively on the file system in your IDE.
        /// - Rename the element in Intent Architect.
        /// - Run the software factory.
        /// </para>
        /// </remarks>
        private (bool Result, string Path) TryGetExistingFilePathInternal()
        {
            var outputPath = FileMetadata.GetFilePath();
            if (File.Exists(outputPath))
            {
                return (true, outputPath);
            }

            var previousOutputPath = ExecutionContext.GetPreviousExecutionLog()?.TryGetFileLog(this)?.FilePath;
            if (previousOutputPath != null &&
                File.Exists(previousOutputPath))
            {
                return (true, previousOutputPath);
            }

            return (false, default);
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
        public bool TryGetExistingFilePath(out string path)
        {
            (var result, path) = _tryGetExistingFilePathCache.Value;

            return result;
        }

        #region TryGetTypeName

        /// <summary>
        /// Obsolete. Use <see cref="TryGetTypeName(string,out string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public string TryGetTypeName(string templateId)
        {
            return TryGetTypeName(templateId, out var typeName)
                ? typeName
                : null;
        }

        /// <summary>
        /// Resolves and applies <see cref="UseType"/> to the type name for the provided
        /// <paramref name="templateId"/> parameter.
        /// Will throw an exception if more than one template instance exists.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <returns><see langword="true"/> if the type name could be resolved.</returns>
        public bool TryGetTypeName(string templateId, out string typeName)
        {
            var classProvider = GetTemplate<IClassProvider>(templateId, TemplateDiscoveryOptions.DoNotThrow);
            if (classProvider == null)
            {
                typeName = null;
                return false;
            }

            typeName = GetTypeName(classProvider);
            return true;
        }

        /// <summary>
        /// Obsolete. Use <see cref="TryGetTypeName(string,IMetadataModel,out string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public string TryGetTypeName(string templateId, IMetadataModel model)
        {
            return TryGetTypeName(templateId, model, out var typeName)
                ? typeName
                : null;
        }

        /// <summary>
        /// Resolves and applies <see cref="UseType"/> to the type name for the provided
        /// <paramref name="templateId"/> parameter.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="model"/>.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="model">The model instance that the Template must be bound to.</param>
        /// <param name="typeName">The resolved type name.</param>
        /// <returns><see langword="true"/> if the type name could be resolved.</returns>
        public bool TryGetTypeName(string templateId, IMetadataModel model, out string typeName)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var classProvider = GetTemplate<IClassProvider>(templateId, model, TemplateDiscoveryOptions.DoNotThrow);
            if (classProvider == null)
            {
                typeName = null;
                return false;
            }

            typeName = GetTypeName(classProvider);
            return true;
        }

        /// <summary>
        /// Obsolete. Use <see cref="TryGetTypeName(string,string,out string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public string TryGetTypeName(string templateId, string modelId)
        {
            return TryGetTypeName(templateId, modelId, out var typeName)
                ? typeName
                : null;
        }

        /// <summary>
        /// Resolves and applies <see cref="UseType"/> to the type name for the provided
        /// <paramref name="templateId"/> parameter.
        /// This overload assumes that the Template can have many instances and identifies the target instance
        /// based on which has the <paramref name="modelId"/>.
        /// <para>
        /// See the
        /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=getTypeName">
        /// Resolving type names</seealso> article for more information.
        /// </para>
        /// </summary>
        /// <param name="templateId">The unique Template identifier.</param>
        /// <param name="modelId">The identifier of the model that the Template must be bound to.</param>
        /// <param name="typeName">The resolved type name.</param>
        /// <returns><see langword="true"/> if the type name could be resolved.</returns>
        public bool TryGetTypeName(string templateId, string modelId, out string typeName)
        {
            var classProvider = GetTemplate<IClassProvider>(templateId, modelId, TemplateDiscoveryOptions.DoNotThrow);
            if (classProvider == null)
            {
                typeName = null;
                return false;
            }

            typeName = GetTypeName(classProvider);
            return true;
        }

        #endregion

        #region GetTemplate

        /// <remarks>
        /// For 4.0.0 we want to add a generic type parameter constraint where <typeparamref name="TTemplate"/>
        /// must be of type <see cref="ITemplate"/>.<br/>
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
        [FixFor_Version4]
        private TTemplate GetTemplate<TTemplate>(
            Func<TTemplate> getTemplate,
            Func<string> getDependencyDescriptionForException,
            TemplateDiscoveryOptions options = null)
        {
            options ??= new TemplateDiscoveryOptions();

            var template = getTemplate();
            if (template == null && options.ThrowIfNotFound)
            {
                throw new Exception(
                    $"Could not find template from dependency: {getDependencyDescriptionForException()}");
            }

            if (options.TrackDependency && template != null)
            {
                AddTemplateDependency((ITemplate)template);
            }

            return template;
        }

        private TTemplate GetTemplate<TTemplate>(ITemplate template, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => template as TTemplate,
                getDependencyDescriptionForException: () => $"{template} as is not a {typeof(TTemplate).Name}",
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

        /// <inheritdoc cref="GetTemplate{TTemplate}(string,TemplateDiscoveryOptions)"/>
        public TTemplate GetTemplate<TTemplate>(string templateId, IMetadataModel model,
            TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () =>
                    ExecutionContext.FindTemplateInstance<TTemplate>(templateId, model.Id),
                getDependencyDescriptionForException: () => $"TemplateId / Role = {templateId}, model.Id = {model.Id}",
                options: options);
        }

        /// <inheritdoc cref="GetTemplate{TTemplate}(string,TemplateDiscoveryOptions)"/>
        public TTemplate GetTemplate<TTemplate>(string templateId, string modelId,
            TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () =>
                    ExecutionContext.FindTemplateInstance<TTemplate>(templateId, modelId),
                getDependencyDescriptionForException: () => $"TemplateId / Role = {templateId}, ModelId = {modelId}",
                options: options);
        }

        /// <summary>
        /// Retrieve an instance of an <see cref="ITemplate"/>. By default an exception will be thrown if the template is not found.
        /// This can be changed in the <see cref="TemplateDiscoveryOptions"/>.
        /// </summary>
        public TTemplate GetTemplate<TTemplate>(string templateId, TemplateDiscoveryOptions options = null)
            where TTemplate : class
        {
            return GetTemplate(
                getTemplate: () => ExecutionContext.FindTemplateInstance<TTemplate>(templateId),
                getDependencyDescriptionForException: () => $"TemplateId / Role = {templateId}",
                options: options);
        }

        /// <summary>
        /// Try to retrieve an instance of <see cref="ITemplate"/>. Returns true if the template is found.
        /// </summary>
        public bool TryGetTemplate<TTemplate>(string templateId, out TTemplate template)
            where TTemplate : class
        {
            template = GetTemplate<TTemplate>(templateId: templateId, options: TemplateDiscoveryOptions.DoNotThrow);
            return template != null;
        }

        /// <inheritdoc cref="TryGetTemplate{TTemplate}(string, out TTemplate)"/>
        public bool TryGetTemplate<TTemplate>(string templateId, IMetadataModel model, out TTemplate template)
            where TTemplate : class
        {
            template = GetTemplate<TTemplate>(templateId: templateId, model: model,
                options: TemplateDiscoveryOptions.DoNotThrow);
            return template != null;
        }

        /// <inheritdoc cref="TryGetTemplate{TTemplate}(string, out TTemplate)"/>
        public bool TryGetTemplate<TTemplate>(string templateId, string modelId, out TTemplate template)
            where TTemplate : class
        {
            template = GetTemplate<TTemplate>(templateId: templateId, modelId: modelId,
                options: TemplateDiscoveryOptions.DoNotThrow);
            return template != null;
        }

        #endregion

        /// <summary>
        /// Returns a string representation of the provided <paramref name="resolvedTypeInfo"/> and
        /// adds any applicable template dependencies. Overrides of this method may perform
        /// additional language specific functionality, such as adding using directives or import
        /// statements.
        /// </summary>
        public virtual string UseType(IResolvedTypeInfo resolvedTypeInfo)
        {
            foreach (var templateDependency in resolvedTypeInfo.GetTemplateDependencies())
            {
                AddTemplateDependency(templateDependency);
            }

            return resolvedTypeInfo.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Id}";
        }
    }
}