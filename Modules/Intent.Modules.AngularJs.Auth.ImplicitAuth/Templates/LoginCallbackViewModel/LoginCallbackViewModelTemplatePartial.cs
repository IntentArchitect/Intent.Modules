using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackViewModel
{
    partial class LoginCallbackViewModelTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.LoginCallbackViewModel";

        public LoginCallbackViewModelTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "LoginCallbackViewModel",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Auth\LoginCallback"
                );
        }
    }
}
