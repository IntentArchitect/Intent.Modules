using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackState
{
    partial class LoginCallbackStateTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginCallbackState";

        public LoginCallbackStateTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "LoginCallbackState",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Auth\LoginCallback"
                );
        }
    }
}
