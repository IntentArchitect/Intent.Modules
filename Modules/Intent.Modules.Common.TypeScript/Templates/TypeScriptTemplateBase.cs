using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Editor;

namespace Intent.Modules.Common.TypeScript.Templates
{
    public abstract class TypeScriptTemplateBase : TypeScriptTemplateBase<object>
    {
        protected TypeScriptTemplateBase(string templateId, IOutputTarget outputTarget, TypescriptTemplateMode mode) : base(templateId, outputTarget, null, mode)
        {
        }
    }

    public abstract class TypeScriptTemplateBase<TModel> : IntentTypescriptProjectItemTemplateBase<TModel>
    {
        protected TypeScriptTemplateBase(string templateId, IOutputTarget outputTarget, TModel model, TypescriptTemplateMode mode) : base(templateId, outputTarget, model)
        {
            TemplateMode = mode;
        }

        public TypescriptTemplateMode TemplateMode { get; }

        public override string RunTemplate()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var file = new TypeScriptFile(source);

            ApplyFileChanges(file);

            file.AddDependencyImports(this);

            return file.GetSource();
        }

        protected virtual void ApplyFileChanges(TypeScriptFile file) { }

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