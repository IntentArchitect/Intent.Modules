using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Typescript.Config.Templates.TypescriptConfig
{
    public partial class TypescriptConfigFileTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Typescript.Config.TypescriptConfig";

        public TypescriptConfigFileTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "tsconfig",
                fileExtension: "json",
                defaultLocationInProject: @"wwwroot\App"
                );
        }
    }
}
