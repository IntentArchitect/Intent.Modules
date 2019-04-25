using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginRedirectState
{
    partial class LoginRedirectStateTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginRedirectState";

        public LoginRedirectStateTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "LoginRedirectState",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Auth\LoginRedirect"
                );
        }
    }
}
