using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Code.Weaving.Kotlin.Editor;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Kotlin.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Kotlin.Templates
{
    /// <inheritdoc />
    public abstract class KotlinTemplateBase : KotlinTemplateBase<object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="KotlinTemplateBase"/>.
        /// </summary>
        protected KotlinTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    /// <inheritdoc cref="KotlinTemplateBase{TModel}"/>
    public abstract class KotlinTemplateBase<TModel, TDecorator> : KotlinTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        /// <summary>
        /// Creates a new instance of <see cref="KotlinTemplateBase{TModel,TDecorator}"/>.
        /// </summary>
        protected KotlinTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        public void AddDecorator(TDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        /// <inheritdoc />
        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        /// <summary>
        /// Aggregates the specified <paramref name="propertyFunc"/> property of all decorators.
        /// </summary>
        /// <remarks>
        /// Ignores Decorators where the property returns null.
        /// </remarks>
        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc)
        {
            return GetDecorators().Aggregate(propertyFunc);
        }

        /// <summary>
        /// Aggregates the specified <paramref name="propertyFunc"/> property of all decorators and
        /// suffixes the result with <paramref name="suffixIfFound"/> provided that there is output
        /// from one or more decorators.
        /// </summary>
        /// <remarks>
        /// Ignores Decorators where the property returns null.
        /// </remarks>
        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc, string suffixIfFound)
        {
            var output = GetDecorators().Aggregate(propertyFunc);
            return string.IsNullOrWhiteSpace(output) ? string.Empty : $"{output}{suffixIfFound}";
        }
    }

    /// <summary>
    /// Template base for Kotlin files, which invokes code-management to make updates to existing files.
    /// </summary>
    public abstract class KotlinTemplateBase<TModel> : IntentTemplateBase<TModel>, IKotlinMerged, IClassProvider, IDeclareImports
    {
        private readonly HashSet<string> _imports = new();

        /// <summary>
        /// Creates a new instance of <see cref="KotlinTemplateBase{TModel}"/>.
        /// </summary>
        protected KotlinTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new KotlinTypeResolver();
        }

        /// <inheritdoc />
        public string ClassName
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("ClassName"))
                {
                    return FileMetadata.CustomMetadata["ClassName"];
                }
                return FileMetadata.FileName;
            }
        }

        /// <summary>
        /// Library dependencies of this template.
        /// </summary>
        public ICollection<KotlinDependency> Dependencies { get; } = new List<KotlinDependency>();

        /// <inheritdoc cref="IFileMetadata.LocationInProject"/>.
        public string Location => FileMetadata.LocationInProject;

        /// <inheritdoc />
        public string Namespace => Package;

        /// <summary>
        /// The package for the type defined by this template.
        /// </summary>
        public string Package
        {
            get
            {
                if (FileMetadata.CustomMetadata.ContainsKey("Package"))
                {
                    return FileMetadata.CustomMetadata["Package"];
                }
                return null;
            }
        }

        /// <summary>
        /// Adds a <see cref="KotlinDependency"/> which can be use by Maven or Gradle to import dependencies.
        /// </summary>
        public void AddDependency(KotlinDependency dependency)
        {
            Dependencies.Add(dependency);
        }

        /// <summary>
        /// Imports the fully qualified type name <paramref name="fullyQualifiedType"/>.
        /// </summary>
        public void AddImport(string fullyQualifiedType)
        {
            if (!_imports.Contains(fullyQualifiedType))
            {
                _imports.Add(fullyQualifiedType);
            }
        }

        /// <inheritdoc />
        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            foreach (var dependency in Dependencies)
            {
                ExecutionContext.EventDispatcher.Publish(dependency);
            }
        }

        /// <inheritdoc />
        protected override ICollectionFormatter CreateCollectionFormatter(string collectionFormat)
        {
            return KotlinCollectionFormatter.GetOrCreate(collectionFormat);
        }

        /// <summary>
        /// Override this method to add additional imports to this Kotlin template. It is recommended to call base.DeclareImports().
        /// </summary>
        public virtual IEnumerable<string> DeclareImports() => _imports;

        /// <summary>
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string. Will automatically import types if necessary.
        /// </summary>
        public override string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            return GetTemplate<IClassProvider>(templateDependency, options).ClassName;
        }

        /// <summary>
        /// Gets the <see cref="KotlinFile"/> of the template output.
        /// </summary>
        public KotlinFile GetTemplateFile()
        {
            return KotlinFile.Parse(base.RunTemplate());
        }

        /// <inheritdoc />
        public override string GetTypeName(ITypeReference typeReference)
        {
            // Overriding as no clear way to handle nullable types when are collections. If built into KotlinTypeResolver, then leads to List<String?> instead of List<String>?
            return $"{base.GetTypeName(typeReference)}{(typeReference.IsNullable ? "?" : "")}";
        }

        /// <inheritdoc />
        public override string GetTypeName(ITypeReference typeReference, string collectionFormat)
        {
            // Overriding as no clear way to handle nullable types when are collections. If built into KotlinTypeResolver, then leads to List<String?> instead of List<String>?
            return $"{base.GetTypeName(typeReference, collectionFormat)}{(typeReference.IsNullable ? "?" : "")}";
        }

        /// <summary>
        /// Imports the fully qualified type and returns its reference name. For example,
        /// java.util.List will import java.util.List and return List.
        /// </summary>
        public string ImportType(string fullyQualifiedType)
        {
            if (fullyQualifiedType.Contains('.'))
            {
                AddImport(fullyQualifiedType);
            }

            return fullyQualifiedType.Split('.').Last();
        }

        /// <inheritdoc />
        public override string RunTemplate()
        {
            var file = GetTemplateFile();

            this.ResolveAndAddImports(file);

            return file.GetSource();
        }

        /// <summary>
        /// Returns a string representation of the provided <paramref name="resolvedTypeInfo"/>,
        /// adds any required imports and applicable template dependencies.
        /// </summary>
        protected override string UseType(IResolvedTypeInfo resolvedTypeInfo)
        {
            if (resolvedTypeInfo is KotlinResolvedTypeInfo { IsPrimitive: false } kotlinResolvedTypeInfo &&
                !string.IsNullOrWhiteSpace(kotlinResolvedTypeInfo.Package))
            {
                foreach (var fullyQualifiedTypeName in kotlinResolvedTypeInfo.GetAllFullyQualifiedTypeNames())
                {
                    AddImport(fullyQualifiedTypeName);
                }
            }

            return base.UseType(resolvedTypeInfo);
        }
    }
}
