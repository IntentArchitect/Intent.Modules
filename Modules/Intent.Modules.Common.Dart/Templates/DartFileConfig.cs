using System;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Dart.Templates
{
    public class DartFileConfig : TemplateFileConfig
    {
        public DartFileConfig(
            string className,
            string fileName = null,
            string relativeLocation = "",
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "dart",
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

