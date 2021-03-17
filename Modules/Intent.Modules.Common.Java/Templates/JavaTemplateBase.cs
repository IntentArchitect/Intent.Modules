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

    public abstract class JavaTemplateBase<TModel> : IntentTemplateBase<TModel>, IJavaMerged, IClassProvider
    {
        protected JavaTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
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

        public void AddTypeSource(string templateId, string collectionFormat = "{0}[]")
        {
            AddTypeSource(JavaTypeSource.Create(ExecutionContext, templateId, collectionFormat));
        }

        public void AddDependency(JavaDependency dependency)
        {
            Dependencies.Add(dependency);
        }

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
    }
}
