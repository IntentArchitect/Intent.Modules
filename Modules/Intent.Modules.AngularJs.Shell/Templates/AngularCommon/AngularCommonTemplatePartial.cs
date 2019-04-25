using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Shell.Templates.AngularCommon
{
    partial class AngularCommonTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Shell.AngularCommon";

        public AngularCommonTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }


        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "Common",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Common"
                );
        }
    }
}
