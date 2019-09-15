using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentProjectItemTemplateBase<TModel> : IntentTemplateBase, ITemplate, ITemplateWithModel, IProjectItemTemplate, IConfigurableTemplate, IHasTemplateDependencies, ITemplatePostConfigurationHook, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public IntentProjectItemTemplateBase(string templateId, IProject project, TModel model)
        {
            Project = project;
            Id = templateId;
            Model = model;
            Context = new TemplateContext(this);
        }

        public string Id { get; }
        public TModel Model { get; }
        public IProject Project { get; }
        public ITemplateContext Context { get; }
        public IFileMetadata FileMetadata { get; private set; }

        object ITemplateWithModel.Model
        {
            get
            {
                return this.Model;
            }
        }

        public void ConfigureFileMetadata(IFileMetadata fileMetadata)
        {
            FileMetadata = fileMetadata;
        }

        public abstract ITemplateFileConfig DefineDefaultFileMetadata();

        public virtual string RunTemplate()
        {
            return TransformText();
        }

        public IFileMetadata GetMetadata()
        {
            return FileMetadata;
        }

        public virtual IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            if (!HasTypeResolver())
            {
                return new List<ITemplateDependency>();
            }
            return Types.GetTemplateDependencies();
        }


        public virtual void OnConfigured()
        {
        }

        public virtual void OnCreated()
        {
        }

        public virtual void BeforeTemplateExecution()
        {
        }
    }

    public class TemplateContext : ITemplateContext
    {
        private object _defaultModelContext;
        private Dictionary<string, object> _prefixLookup;

        public TemplateContext(object defaultModelContext)
        {
            _defaultModelContext = defaultModelContext;
        }

        public TemplateContext() : this(null)
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

        public object GetRootContext(string propertyName, out bool isDefault)
        {
            if (_prefixLookup != null && _prefixLookup.ContainsKey(propertyName))
            {
                isDefault = false;
                return _prefixLookup[propertyName];
            }
            else
            {
                isDefault = true;
                return _defaultModelContext;
            }

        }
    }

    public abstract class IntentProjectItemTemplateBase : IntentProjectItemTemplateBase<object>
    {
        public IntentProjectItemTemplateBase(string templateId, IProject project) : base(templateId, project, null)
        {

        }

    }
}
