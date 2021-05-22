using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Code.Weaving.Java.Editor;
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

        public string Location => FileMetadata.LocationInProject;
        public ICollection<JavaDependency> Dependencies { get; } = new List<JavaDependency>();

        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public new void AddTypeSource(string templateId, string collectionFormat = "{0}[]")
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, collectionFormat));
        }

        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public void AddTypeSource(string templateId, Func<string, string> formatCollection)
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, new CollectionFormatter(formatCollection)));
        }

        [Obsolete("Specify using fluent api (e.g. AddTypeSource(...).WithCollectionFormat(...);")]
        public void AddTypeSource(string templateId, ICollectionFormatter collectionFormatter)
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, collectionFormatter));
        }

        /// <summary>
        /// Adds the <see cref="JavaDependency"/> which can be use by Maven or Gradle to import dependencies.
        /// </summary>
        /// <param name="dependency"></param>
        public void AddDependency(JavaDependency dependency)
        {
            Dependencies.Add(dependency);
        }

        /// <summary>
        /// imports the fully qualified type and returns its reference name. For example, java.util.List will import java.util.List and return List.
        /// </summary>
        /// <param name="fullyQualifiedType"></param>
        /// <returns></returns>
        public string ImportType(string fullyQualifiedType)
        {
            return ImportType(fullyQualifiedType.Split('.').Last(), fullyQualifiedType);
        }

        private string ImportType(string typeName, string import)
        {
            if (!_imports.Contains(import))
            {
                _imports.Add(import);
            }

            return typeName;
        }

        /// <summary>
        /// Resolves the type name of the <paramref name="templateDependency"/> as a string. Will automatically import types if necessary.
        /// </summary>
        /// <param name="templateDependency"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override string GetTypeName(ITemplateDependency templateDependency, TemplateDiscoveryOptions options = null)
        {
            return GetTemplate<IClassProvider>(templateDependency, options).ClassName;
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            foreach (var dependency in Dependencies)
            {
                ExecutionContext.EventDispatcher.Publish(dependency);
            }
        }

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

        public JavaFile GetExistingFile()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
            return File.Exists(fullFileName) ? JavaFile.Parse(File.ReadAllText(fullFileName)) : null;
        }

        /// <summary>
        /// Override this method to add additional imports to this java template. It is recommended to call base.DeclareImports().
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> DeclareImports() => _imports;
    }
}
