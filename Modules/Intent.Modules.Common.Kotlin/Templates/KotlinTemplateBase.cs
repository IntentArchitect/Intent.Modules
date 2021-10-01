using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Code.Weaving.Kotlin.Editor;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Kotlin.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Kotlin.Templates
{
    public abstract class KotlinTemplateBase : KotlinTemplateBase<object>
    {
        protected KotlinTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    public abstract class KotlinTemplateBase<TModel, TDecorator> : KotlinTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        protected KotlinTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
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

        protected string GetDecoratorsOutput(Func<TDecorator, string> propertyFunc, string suffixIfFound)
        {
            var output = GetDecorators().Aggregate(propertyFunc);
            return string.IsNullOrWhiteSpace(output) ? string.Empty : $"{output}{suffixIfFound}";
        }
    }

    public abstract class KotlinTemplateBase<TModel> : IntentTemplateBase<TModel>, IKotlinMerged, IClassProvider, IDeclareImports
    {
        private readonly ICollection<string> _imports = new List<string>();

        protected KotlinTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new KotlinTypeResolver();
        }

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

        public string Namespace => Package;

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

        /// <inheritdoc cref="IFileMetadata.LocationInProject"/>.
        public string Location => FileMetadata.LocationInProject;

        public ICollection<KotlinDependency> Dependencies { get; } = new List<KotlinDependency>();

        /// <summary>
        /// Adds a <see cref="KotlinDependency"/> which can be use by Maven or Gradle to import dependencies.
        /// </summary>
        public void AddDependency(KotlinDependency dependency)
        {
            Dependencies.Add(dependency);
        }

        /// <summary>
        /// Imports the fully qualified type and returns its reference name. For example,
        /// java.util.List will import java.util.List and return List.
        /// </summary>
        public string ImportType(string fullyQualifiedType)
        {
            AddImport(fullyQualifiedType);

            return fullyQualifiedType.Split('.').Last();
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

        /// <summary>
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string. Will automatically import types if necessary.
        /// </summary>
        public override string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            return GetTemplate<IClassProvider>(templateDependency, options).ClassName;
        }

        // Overriding as no clear way to handle nullable types when are collections. If built into KotlinTypeResolver, then leads to List<String?> instead of List<String>?
        /// <inheritdoc />
        public override string GetTypeName(ITypeReference typeReference, string collectionFormat)
        {
            return $"{base.GetTypeName(typeReference, collectionFormat)}{(typeReference.IsNullable ? "?" : "")}";
        }

        // Overriding as no clear way to handle nullable types when are collections. If built into KotlinTypeResolver, then leads to List<String?> instead of List<String>?
        /// <inheritdoc />
        public override string GetTypeName(ITypeReference typeReference)
        {
            return $"{base.GetTypeName(typeReference)}{(typeReference.IsNullable ? "?" : "")}";
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
        public override string RunTemplate()
        {
            var file = CreateOutputFile();

            this.ResolveAndAddImports(file);

            return file.GetSource();
        }

        protected virtual KotlinFile CreateOutputFile()
        {
            return GetTemplateFile();
        }

        public KotlinFile GetTemplateFile()
        {
            return KotlinFile.Parse(base.RunTemplate());
        }

        public KotlinFile GetExistingFile()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
            return File.Exists(fullFileName) ? KotlinFile.Parse(File.ReadAllText(fullFileName)) : null;
        }

        /// <summary>
        /// Override this method to add additional imports to this Kotlin template. It is recommended to call base.DeclareImports().
        /// </summary>
        public virtual IEnumerable<string> DeclareImports() => _imports;
    }
}
