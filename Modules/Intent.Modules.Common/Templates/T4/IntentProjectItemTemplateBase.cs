using Intent.SoftwareFactory.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
{
    public abstract class IntentProjectItemTemplateBase<TModel> : IntentTemplateBase, ITemplate, ITemplateWithModel, IProjectItemTemplate, IConfigurableTemplate
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
        public TemplateContext Context { get; }
        public IFileMetaData FileMetaData { get; private set; }

        public virtual string DependencyUsings => this.ResolveAllUsings(Project);

        object ITemplateWithModel.Model
        {
            get
            {
                return this.Model;
            }
        }

        public void ConfigureFileMetaData(IFileMetaData fileMetaData)
        {
            FileMetaData = fileMetaData;
        }

        public abstract DefaultFileMetaData DefineDefaultFileMetaData();

        public virtual string RunTemplate()
        {
            return TransformText();
        }

        public IFileMetaData GetMetaData()
        {
            return FileMetaData;
        }
    }

    public abstract class IntentProjectItemTemplateBase : IntentProjectItemTemplateBase<object>
    {
        public IntentProjectItemTemplateBase(string templateId, IProject project) : base(templateId, project, null)
        {

        }

    }
}
