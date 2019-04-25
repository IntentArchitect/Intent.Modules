using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackView
{
    partial class LoginCallbackViewTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginCallbackView";

        public LoginCallbackViewTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "LoginCallback",
                fileExtension: "html",
                defaultLocationInProject: @"wwwroot\App\Auth\LoginCallback"
                );
        }
    }
}
