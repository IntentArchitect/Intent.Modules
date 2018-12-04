using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginRedirectState
{
    partial class LoginRedirectStateTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginRedirectState";

        public LoginRedirectStateTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
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
