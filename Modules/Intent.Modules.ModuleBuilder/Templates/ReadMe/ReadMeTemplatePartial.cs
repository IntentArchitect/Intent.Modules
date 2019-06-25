using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;
namespace Intent.Modules.ModuleBuilder.Templates.ReadMe
{
    partial class ReadMeTemplate : IntentProjectItemTemplateBase
    {
        public const string TemplateId = "Intent.ModuleBuilder.ReadMe";

        public ReadMeTemplate(IProject project) : base(TemplateId, project)
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