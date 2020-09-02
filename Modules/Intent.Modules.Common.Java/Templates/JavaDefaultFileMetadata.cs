using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeScript.Templates
{
    public class JavaDefaultFileMetadata : DefaultFileMetadata
    {
        public JavaDefaultFileMetadata(
            OverwriteBehaviour overwriteBehaviour,
            string fileName,
            string defaultLocationInProject,
            string className,
            string @namespace,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "java"
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
        }
    }
}