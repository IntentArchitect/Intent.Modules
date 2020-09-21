using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Html.Templates
{
    public class HtmlDefaultFileMetadata : DefaultFileMetadata
    {
        public HtmlDefaultFileMetadata(
            OverwriteBehaviour overwriteBehaviour,
            string fileName,
            string relativeLocation,
            string codeGenType = Common.CodeGenType.Basic,
            string fileExtension = "html"
        ) : base(overwriteBehaviour: overwriteBehaviour,
                codeGenType: codeGenType,
                fileName: fileName,
                fileExtension: fileExtension,
                defaultLocationInProject: relativeLocation)
        {
        }
    }
}