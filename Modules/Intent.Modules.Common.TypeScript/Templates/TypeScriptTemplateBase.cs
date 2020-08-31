using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Editor;

namespace Intent.Modules.Common.TypeScript.Templates
{
    public interface ITypeScriptMerged
    {
        TypeScriptFile GetExistingFile();
    }

    public abstract class TypeScriptTemplateBase : TypeScriptTemplateBase<object>
    {
        protected TypeScriptTemplateBase(string templateId, IProject outputTarget) : base(templateId, outputTarget, null)
        {
        }
    }

    public abstract class TypeScriptTemplateBase<TModel> : IntentTypescriptProjectItemTemplateBase<TModel>, ITypeScriptMerged
    {
        protected TypeScriptTemplateBase(string templateId, IProject outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
        }

        public override string RunTemplate()
        {
            var file = CreateOutputFile();

            file.AddDependencyImports(this);

            return file.GetSource();
        }

        protected virtual TypeScriptFile CreateOutputFile()
        {
            return GetTemplateFile();
        }

        public TypeScriptFile GetTemplateFile()
        {
            return new TypeScriptFile(base.RunTemplate());
        }

        public TypeScriptFile GetExistingFile()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
            return File.Exists(fullFileName) ? new TypeScriptFile(File.ReadAllText(fullFileName)) : null;
        }
    }
}