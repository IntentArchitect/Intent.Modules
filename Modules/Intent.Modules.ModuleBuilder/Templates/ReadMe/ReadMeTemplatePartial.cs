using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;
namespace Intent.Modules.ModuleBuilder.Templates.ReadMe
{
    partial class ReadMeTemplate : IntentTemplateBase
    {
        public const string TemplateId = "Intent.ModuleBuilder.ReadMe";

        public ReadMeTemplate(IOutputContext context) : base(TemplateId, context)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "README",
                fileExtension: "txt",
                defaultLocationInProject: ""
            );
        }
    }
}