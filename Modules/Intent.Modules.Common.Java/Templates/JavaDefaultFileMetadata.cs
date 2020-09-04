using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    public class JavaDefaultFileMetadata : DefaultFileMetadata
    {
        public JavaDefaultFileMetadata(
            OverwriteBehaviour overwriteBehaviour,
            string fileName,
            string relativeLocation,
            string className,
            string package = null,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "java"
        )
            : base(overwriteBehaviour: overwriteBehaviour, 
                codeGenType: codeGenType, 
                fileName: fileName, 
                fileExtension: fileExtension,
                defaultLocationInProject: relativeLocation)
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                this.CustomMetadata["ClassName"] = className;
            }
            if (!string.IsNullOrWhiteSpace(package))
            {
                this.CustomMetadata["Package"] = package;
            }
        }
    }
}