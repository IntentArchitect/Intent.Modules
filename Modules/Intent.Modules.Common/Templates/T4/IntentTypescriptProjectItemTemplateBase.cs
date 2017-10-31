using Intent.SoftwareFactory.Engine;

namespace Intent.SoftwareFactory.Templates
{
    public abstract class IntentTypescriptProjectItemTemplateBase<TModel> : IntentProjectItemTemplateBase<TModel>, IHasClassDetails
    {
        public IntentTypescriptProjectItemTemplateBase(string templateId, IProject project, TModel model) : base(templateId, project, model)
        {
        }

        public string Namespace
        {
            get
            {
                if (FileMetaData.CustomMetaData.ContainsKey("Namespace"))
                {
                    return FileMetaData.CustomMetaData["Namespace"];
                }
                return this.Project.Name;
            }
        }

        public string ClassName
        {
            get
            {
                if (FileMetaData.CustomMetaData.ContainsKey("ClassName"))
                {
                    return FileMetaData.CustomMetaData["ClassName"];
                }
                return FileMetaData.FileName;
            }
        }

        public sealed override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return DefineTypescriptDefaultFileMetaData();
        }

        protected abstract TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData();
    }

    public class TypescriptDefaultFileMetaData : DefaultFileMetaData
    {
        public TypescriptDefaultFileMetaData(
                    OverwriteBehaviour overwriteBehaviour,
                    string codeGenType,
                    string fileName,
                    string fileExtension,
                    string defaultLocationInProject,
                    string className,
                    string @namespace,
                    string dependsUpon = null
                    )
            : base(overwriteBehaviour, codeGenType, fileName, fileExtension, defaultLocationInProject)
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                this.CustomMetaData["ClassName"] = className;
            }
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                this.CustomMetaData["Namespace"] = @namespace;
            }
            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                this.CustomMetaData["Depends On"] = dependsUpon;
            }
        }
    }
}