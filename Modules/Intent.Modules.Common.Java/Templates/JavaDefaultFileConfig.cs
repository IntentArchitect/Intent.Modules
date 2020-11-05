using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Java.Templates
{
    public class JavaDefaultFileConfig : DefaultFileMetadata
    {
        public JavaDefaultFileConfig(
            string className,
            string package,
            string relativeLocation = "",
            string fileName = null,
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "java"
        )
            : base(overwriteBehaviour: overwriteBehaviour, 
                codeGenType: codeGenType, 
                fileName: fileName ?? className, 
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