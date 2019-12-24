using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
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
                if (FileMetadata.CustomMetadata.ContainsKey("Namespace"))
                {
                    return FileMetadata.CustomMetadata["Namespace"];
                }
                return this.Project.Name;
            }
        }

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

        //public sealed override ITemplateFileConfig DefineDefaultFileMetadata()
        //{
        //    return DefineTypescriptDefaultFileMetadata();
        //}

        //protected abstract TypescriptDefaultFileMetadata DefineTypescriptDefaultFileMetadata();
    }

    public class TypescriptDefaultFileMetadata : DefaultFileMetadata
    {
        public TypescriptDefaultFileMetadata(
                    OverwriteBehaviour overwriteBehaviour,
                    string codeGenType,
                    string fileName,
                    string fileExtension,
                    string defaultLocationInProject,
                    string className,
                    string @namespace = null,
                    string dependsUpon = null
                    )
            : base(overwriteBehaviour: overwriteBehaviour, 
                  codeGenType: codeGenType, 
                  fileName: fileName, 
                  fileExtension: fileExtension,
                  defaultLocationInProject: defaultLocationInProject)
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                this.CustomMetadata["ClassName"] = className;
            }
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                this.CustomMetadata["Namespace"] = @namespace;
            }
            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                this.CustomMetadata["Depends On"] = dependsUpon;
            }
        }
    }
}