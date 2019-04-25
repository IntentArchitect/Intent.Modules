using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.AspNetCore.Docker.Templates.DockerIgnore
{
    partial class DockerIgnoreTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AspNetCore.DockerIgnore";


        public DockerIgnoreTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }


        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: ".dockerignore",
                fileExtension: "",
                defaultLocationInProject: ""
                );
        }
    }
}
