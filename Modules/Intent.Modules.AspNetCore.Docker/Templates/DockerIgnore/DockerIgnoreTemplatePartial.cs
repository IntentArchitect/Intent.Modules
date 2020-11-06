using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Docker.Templates.DockerIgnore
{
    partial class DockerIgnoreTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AspNetCore.DockerIgnore";


        public DockerIgnoreTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }


        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: ".dockerignore",
                fileExtension: "",
                relativeLocation: ""
                );
        }
    }
}
