using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Typescript.Config.Templates.TypescriptDefinitelyTypedReference
{
    public partial class TypescriptDefinitelyTypedReferencesTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Typescript.Config.DefinitelyTypedReferences";

        public TypescriptDefinitelyTypedReferencesTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "_references",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App"
                );
        }
    }
}
