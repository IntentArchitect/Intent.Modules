using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeScript.Templates
{
    public class TypeScriptFileConfig : TemplateFileConfig
    {
        public TypeScriptFileConfig(
            string className,
            string fileName = null,
            string relativeLocation = "",
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "ts",
            string @namespace = null
        ) : base(fileName: fileName ?? className.ToKebabCase(),
                fileExtension: fileExtension,
                relativeLocation: relativeLocation,
                overwriteBehaviour: overwriteBehaviour,
                codeGenType: codeGenType)
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