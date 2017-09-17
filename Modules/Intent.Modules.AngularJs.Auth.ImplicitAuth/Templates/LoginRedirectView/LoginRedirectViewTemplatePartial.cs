using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginRedirectView
{
    partial class LoginRedirectViewTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginRedirectView";

        public LoginRedirectViewTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "LoginRedirect",
                fileExtension: "html",
                defaultLocationInProject: @"wwwroot\App\Auth\LoginRedirect"
                );
        }
    }
}
