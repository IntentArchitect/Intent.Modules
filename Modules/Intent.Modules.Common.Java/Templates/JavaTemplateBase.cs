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
    public abstract class JavaTemplateBase : JavaTemplateBase<object>
    {
        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    public abstract class JavaTemplateBase<TModel, TDecorator> : JavaTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        public IEnumerable<TDecorator> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        /// <inheritdoc />
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

    public abstract class JavaTemplateBase<TModel> : IntentTemplateBase<TModel>, IJavaMerged, IClassProvider, IDeclareImports
    {
        private readonly ICollection<string> _imports = new List<string>();

        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new JavaTypeResolver();
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

        /// <inheritdoc />
        public string Namespace => Package;

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

        public string Location => FileMetadata.LocationInProject;
        public ICollection<JavaDependency> Dependencies { get; } = new List<JavaDependency>();

        /// <summary>
        /// Obsolete. Specify using fluent api (e.g. `AddTypeSource(...).WithCollectionFormat(...);`.
        /// </summary>
        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public new void AddTypeSource(string templateId, string collectionFormat)
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, collectionFormat));
        }

        /// <summary>
        /// Obsolete. Specify using fluent api (e.g. `AddTypeSource(...).WithCollectionFormat(...);`.
        /// </summary>
        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public void AddTypeSource(string templateId, Func<string, string> formatCollection)
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, new CollectionFormatter(formatCollection)));
        }

        /// <summary>
        /// Obsolete. Specify using fluent api (e.g. `AddTypeSource(...).WithCollectionFormat(...);`.
        /// </summary>
        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public void AddTypeSource(string templateId, ICollectionFormatter collectionFormatter)
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, collectionFormatter));
        }

        /// <summary>
        /// Adds the <see cref="JavaDependency"/> which can be use by Maven or Gradle to import
        /// dependencies.
        /// </summary>
        public void AddDependency(JavaDependency dependency)
        {
            Dependencies.Add(dependency);
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
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string.
        /// Will automatically import types if necessary.
        /// </summary>
        public override string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            return GetTemplate<IClassProvider>(templateDependency, options).ClassName;
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

        protected virtual JavaFile CreateOutputFile()
        {
            return GetTemplateFile();
        }

        public JavaFile GetTemplateFile()
        {
            return JavaFile.Parse(base.RunTemplate());
        }

        /// <summary>
        /// Override this method to add additional imports to this Java template. It is
        /// recommended to call base.DeclareImports().
        /// </summary>
        public virtual IEnumerable<string> DeclareImports() => _imports;
    }
}
