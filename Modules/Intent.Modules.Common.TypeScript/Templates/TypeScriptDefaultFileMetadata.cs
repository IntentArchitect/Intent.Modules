using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeScript.Templates
{
    public class TypeScriptDefaultFileMetadata : DefaultFileMetadata
    {
        public TypeScriptDefaultFileMetadata(
            OverwriteBehaviour overwriteBehaviour,
            string fileName,
            string defaultLocationInProject,
            string className,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "ts",
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