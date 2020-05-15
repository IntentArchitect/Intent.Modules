using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Angular
{
    public abstract class AngularTypescriptProjectItemTemplateBase : AngularTypescriptProjectItemTemplateBase<object>
    {
        protected AngularTypescriptProjectItemTemplateBase(string templateId, IProject project, TypescriptTemplateMode mode) : base(templateId, project, null, mode)
        {
        }
    }

    public abstract class AngularTypescriptProjectItemTemplateBase<TModel> : IntentTypescriptProjectItemTemplateBase<TModel>
    {
        protected AngularTypescriptProjectItemTemplateBase(string templateId, IProject project, TModel model, TypescriptTemplateMode mode) : base(templateId, project, model)
        {
            TemplateMode = mode;
        }

        public TypescriptTemplateMode TemplateMode { get; }

        public override string RunTemplate()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var file = new TypescriptFile(source);

            ApplyFileChanges(file);

            file.AddDependencyImports(this);

            return file.GetChangedSource();
        }

        protected virtual void ApplyFileChanges(TypescriptFile file) { }

        protected string LoadOrCreate(string fullFileName)
        {
            if (TemplateMode == TypescriptTemplateMode.AlwaysRecreateFromTemplate)
            {
                return base.RunTemplate();
            }
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : base.RunTemplate();
        }
    }

    public enum TypescriptTemplateMode
    {
        AlwaysRecreateFromTemplate = 0,
        UpdateFile = 1
    }
}