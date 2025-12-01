using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Html.Templates
{
    public class HtmlFileConfig : TemplateFileConfig
    {
        public HtmlFileConfig(
            string fileName,
            string relativeLocation,
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "html"
        ) : base(overwriteBehaviour: overwriteBehaviour,
                codeGenType: codeGenType,
                fileName: fileName,
                fileExtension: fileExtension,
                relativeLocation: relativeLocation)
        {
        }
    }
}