using System;
using System.Collections.Generic;
using System.Linq;
//using Intent.Code.Weaving.TypeScript.Editor;
using Intent.Engine;
using Intent.Modules.Common.Dart.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Dart.Templates;
using Intent.Templates;
using Intent.Utils;

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
    /// Template base for TypeScript files, which invokes code-management to make updates to existing files.
    /// </summary>
    public abstract class DartTemplateBase<TModel> : IntentDartProjectItemTemplateBase<TModel>, IDartMerged
    {
        private readonly HashSet<DartImport> _imports = new();

        /// <summary>
        /// Creates a new instance of <see cref="DartTemplateBase{TModel}"/>.
        /// </summary>
        protected DartTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
            Types = new DartTypeResolver();
        }

        /// <summary>
        /// (Type/Java)Script Imports for the generated file.
        /// </summary>
        public ICollection<DartImport> Imports => _imports;

        /// <summary>
        /// NPM package dependencies for this template.
        /// </summary>
        public ICollection<NpmPackageDependency> Dependencies { get; } = new List<NpmPackageDependency>();

        /// <summary>
        /// Adds the <see cref="NpmPackageDependency"/> which can be use by Intent.Npm to import dependencies.
        ///// </summary>
        public void AddDependency(NpmPackageDependency dependency)
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

        ///// <summary>
        ///// Gets the <see cref="TypeScriptFile"/> of the template output.
        ///// </summary>
        //public DartFile GetTemplateFile()
        //{
        //    try
        //    {
        //        return new TypeScriptFileEditor(base.RunTemplate()).File;
        //    }
        //    catch
        //    {
        //        Logging.Log.Failure($@"Failed to parse TypesScript output file:
        //{base.RunTemplate()}");
        //        throw;
        //    }
        //}

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
            if (fullyQualifiedType.Contains("<") && fullyQualifiedType.Contains(">"))
            {
                var genericTypes = fullyQualifiedType.Substring(fullyQualifiedType.IndexOf("<", StringComparison.Ordinal) + 1, fullyQualifiedType.Length - fullyQualifiedType.IndexOf("<", StringComparison.Ordinal) - 2);

                normalizedGenericTypes = genericTypes
                    .Split(',')
                    .Select(NormalizeTypeName)
                    .Aggregate((x, y) => x + ", " + y);
                fullyQualifiedType = $"{fullyQualifiedType.Substring(0, fullyQualifiedType.IndexOf("<", StringComparison.Ordinal))}";
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
            return string.Join(".", typeParts) + (normalizedGenericTypes != null ? $"<{normalizedGenericTypes}>" : "");
        }

        /// <inheritdoc />
        //public override string RunTemplate()
        //{
        //    var file = GetTemplateFile();

        //    file.AddDependencyImports(this);

        //    return file.GetSource();
        //}
    }
}

