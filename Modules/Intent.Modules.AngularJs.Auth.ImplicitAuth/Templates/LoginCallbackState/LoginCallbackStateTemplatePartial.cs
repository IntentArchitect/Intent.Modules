using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackState
{
    partial class LoginCallbackStateTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginCallbackState";

        public LoginCallbackStateTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "LoginCallbackState",
                fileExtension: "ts",
                defaultLocationInProject: "wwwroot/App/Auth/LoginCallback"
                );
        }
    }
}
