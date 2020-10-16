using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Typescript.Config.Templates.TypescriptConfig
{
    public partial class TypescriptConfigFileTemplate : IntentTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Typescript.Config.TypescriptConfig";

        public TypescriptConfigFileTemplate(IOutputTarget project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "tsconfig",
                fileExtension: "json",
                defaultLocationInProject: "wwwroot/App"
                );
        }
    }
}
