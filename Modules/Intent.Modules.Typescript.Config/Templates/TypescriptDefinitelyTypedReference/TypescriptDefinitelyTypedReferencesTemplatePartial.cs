using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Typescript.Config.Templates.TypescriptDefinitelyTypedReference
{
    public partial class TypescriptDefinitelyTypedReferencesTemplate : IntentTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Typescript.Config.DefinitelyTypedReferences";

        public TypescriptDefinitelyTypedReferencesTemplate(IOutputTarget project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "_references",
                fileExtension: "ts",
                defaultLocationInProject: "wwwroot/App"
                );
        }
    }
}
