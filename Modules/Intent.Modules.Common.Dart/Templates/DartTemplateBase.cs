using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Dart.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Dart.Templates
{
    /// <inheritdoc />
    public abstract class DartTemplateBase : DartTemplateBase<object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DartTemplateBase"/>.
        /// </summary>
        protected DartTemplateBase(string templateId, IOutputTarget outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    /// <inheritdoc cref="DartTemplateBase{TModel}"/>
    public abstract class DartTemplateBase<TModel, TDecorator> : DartTemplateBase<TModel>, IHasDecorators<TDecorator>
        where TDecorator : ITemplateDecorator
    {
        private readonly ICollection<TDecorator> _decorators = new List<TDecorator>();

        /// <summary>
        /// Creates a new instance of <see cref="DartTemplateBase{TModel,TDecorator}"/>.
        /// </summary>
        protected DartTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
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
    }

    /// <summary>
    /// Template base for Dart files.
    /// </summary>
    public abstract class DartTemplateBase<TModel> : IntentTemplateBase<TModel>, IClassProvider
    {
        private readonly HashSet<DartImport> _imports = new();

        /// <summary>
        /// Creates a new instance of <see cref="DartTemplateBase{TModel}"/>.
        /// </summary>
        protected DartTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new DartTypeResolver();
        }

        /// <inheritdoc />
        public string Namespace => string.Empty;

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

        /// <inheritdoc />
        public override string UseType(IResolvedTypeInfo resolvedTypeInfo)
        {
            var normalizedTypeName = NormalizeTypeName(resolvedTypeInfo.ToString());

            return normalizedTypeName;
        }

        /// <summary>
        /// Package dependencies for this template.
        /// </summary>
        public ICollection<DartPackageDependency> Dependencies { get; } = new List<DartPackageDependency>();

        /// <summary>
        /// Adds an instance of <see cref="DartPackageDependency"/> to <see cref="Dependencies"/>.
        /// </summary>
        public void AddDependency(DartPackageDependency dependency)
        {
            Dependencies.Add(dependency);
        }

        /// <summary>
        /// Adds an import with the specified information to this template.
        /// </summary>
        public void AddImport(string type, string location)
        {
            _imports.Add(new DartImport(type, location));
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

        /// <summary>
        /// Adds an import with the specified information to this template and returns the
        /// <paramref name="type"/>.
        /// </summary>
        public string ImportType(string type, string location)
        {
            AddImport(type, location);
            return type;
        }

        /// <inheritdoc />
        public override string NormalizeTypeName(string fullyQualifiedType)
        {
            if (string.IsNullOrWhiteSpace(Namespace))
            {
                return fullyQualifiedType;
            }

            string normalizedGenericTypes = null;
            if (fullyQualifiedType.Contains('<') && fullyQualifiedType.Contains('>'))
            {
                var genericTypes = fullyQualifiedType.Substring(fullyQualifiedType.IndexOf('<', StringComparison.Ordinal) + 1, fullyQualifiedType.Length - fullyQualifiedType.IndexOf('<', StringComparison.Ordinal) - 2);

                normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeTypeName)
                    .Aggregate((x, y) => x + ", " + y);
                fullyQualifiedType = $"{fullyQualifiedType[..fullyQualifiedType.IndexOf('<', StringComparison.Ordinal)]}";
            }

            var typeParts = fullyQualifiedType.Split('.').ToList();
            var localNamespaceParts = Namespace.Split('.').ToList();

            foreach (var part in localNamespaceParts)
            {
                if (part.Equals(typeParts[0]))
                {
                    typeParts.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            return string.Join('.', typeParts) + (normalizedGenericTypes != null ? $"<{normalizedGenericTypes}>" : string.Empty);
        }
    }
}

