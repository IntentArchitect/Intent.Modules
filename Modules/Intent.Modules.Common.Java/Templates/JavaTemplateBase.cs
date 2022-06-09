using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Code.Weaving.Java.Editor;
using Intent.Engine;
using Intent.Modules.Common.Java.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    /// <inheritdoc />
    public abstract class JavaTemplateBase : JavaTemplateBase<object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="JavaTemplateBase"/>.
        /// </summary>
        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    /// <inheritdoc cref="JavaTemplateBase{TModel}"/>
    public abstract class JavaTemplateBase<TModel, TDecorator> : JavaTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        /// <summary>
        /// Creates a new instance of <see cref="JavaTemplateBase{TModel,TDecorator}"/>.
        /// </summary>
        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
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
    /// Template base for Java files, which invokes code-management to make updates to existing files.
    /// </summary>
    public abstract class JavaTemplateBase<TModel> : IntentTemplateBase<TModel>, IJavaMerged, IClassProvider, IDeclareImports
    {
        private readonly HashSet<string> _imports = new();

        /// <summary>
        /// Creates a new instance of <see cref="JavaTemplateBase{TModel}"/>.
        /// </summary>
        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new JavaTypeResolver();
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
        public ICollection<JavaDependency> Dependencies { get; } = new List<JavaDependency>();

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

        string IClassProvider.Namespace => Package;

        /// <summary>
        /// Adds the <see cref="JavaDependency"/> which can be use by Maven or Gradle to import
        /// dependencies.
        /// </summary>
        public void AddDependency(JavaDependency dependency)
        {
            Dependencies.Add(dependency);
        }

        /// <summary>
        /// Imports the fully qualified type name <paramref name="fullyQualifiedType"/>.
        /// </summary>
        public void AddImport(string fullyQualifiedType)
        {
            _imports.Add(fullyQualifiedType);
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
            return JavaCollectionFormatter.GetOrCreate(collectionFormat);
        }

        /// <summary>
        /// Override this method to add additional imports to this Java template. It is
        /// recommended to call base.DeclareImports().
        /// </summary>
        public virtual IEnumerable<string> DeclareImports() => _imports;

        /// <summary>
        /// Gets the <see cref="JavaFile"/> of the template output.
        /// </summary>
        public JavaFile GetTemplateFile()
        {
            return JavaFile.Parse(base.RunTemplate());
        }

        /// <summary>
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string.
        /// Will automatically import types if necessary.
        /// </summary>
        public override string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            return GetTemplate<IClassProvider>(templateDependency, options).ClassName;
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
        public override string NormalizeTypeName(string fullyQualifiedType)
        {
            string normalizedGenericTypes = null;
            if (fullyQualifiedType.Contains("<") && fullyQualifiedType.Contains(">"))
            {
                var genericTypes = fullyQualifiedType.Substring(fullyQualifiedType.IndexOf("<", StringComparison.Ordinal) + 1, fullyQualifiedType.Length - fullyQualifiedType.IndexOf("<", StringComparison.Ordinal) - 2);

                normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeTypeName)
                    .Aggregate((x, y) => x + ", " + y);
                fullyQualifiedType = $"{fullyQualifiedType.Substring(0, fullyQualifiedType.IndexOf("<", StringComparison.Ordinal))}";
            }
            return ImportType(fullyQualifiedType) + (normalizedGenericTypes != null ? $"<{normalizedGenericTypes}>" : "");
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
            if (resolvedTypeInfo is JavaResolvedTypeInfo { IsPrimitive: false } javaResolvedTypeInfo &&
                !string.IsNullOrWhiteSpace(javaResolvedTypeInfo.Package))
            {
                foreach (var fullyQualifiedTypeName in javaResolvedTypeInfo.GetAllFullyQualifiedTypeNames())
                {
                    AddImport(fullyQualifiedTypeName);
                }
            }

            return base.UseType(resolvedTypeInfo);
        }
    }
}
