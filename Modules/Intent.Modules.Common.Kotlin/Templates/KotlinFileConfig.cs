using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Kotlin.Templates
{
    public class KotlinFileConfig : TemplateFileConfig
    {
        public KotlinFileConfig(
            string className,
            string package,
            string relativeLocation = "",
            string fileName = null,
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "kt"
        )
            : base(overwriteBehaviour: overwriteBehaviour, 
                codeGenType: codeGenType, 
                fileName: fileName ?? className, 
                fileExtension: fileExtension,
                relativeLocation: relativeLocation)
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