using System.Linq;
using Intent.MetaModel.Hosting;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.AuthModule;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.HttpInterceptor;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackState;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackView;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.LoginCallbackViewModel;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.LoginRedirectState;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.LoginRedirectView;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.SilentRenewCallbackView;
using Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.UserInfoService;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;

namespace Intent.AngularJs.Shell
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var hostingConfig = metaDataManager.GetMetaData<HostingConfigModel>("LocalHosting").SingleOrDefault(x => x.ApplicationName == application.ApplicationName);

            RegisterTemplate(AngularImplicitAuthModuleTemplate.Identifier, project => new AngularImplicitAuthModuleTemplate(project, hostingConfig, application.EventDispatcher, application.SolutionEventDispatcher));
            RegisterTemplate(AngularAuthHttpInterceptorServiceTemplate.Identifier, project => new AngularAuthHttpInterceptorServiceTemplate(project));
            RegisterTemplate(LoginCallbackStateTemplate.Identifier, project => new LoginCallbackStateTemplate(project));
            RegisterTemplate(LoginCallbackViewTemplate.Identifier, project => new LoginCallbackViewTemplate(project));
            RegisterTemplate(LoginCallbackViewModelTemplate.Identifier, project => new LoginCallbackViewModelTemplate(project));
            RegisterTemplate(LoginRedirectStateTemplate.Identifier, project => new LoginRedirectStateTemplate(project));
            RegisterTemplate(LoginRedirectViewTemplate.Identifier, project => new LoginRedirectViewTemplate(project));
            RegisterTemplate(SilentRenewCallbackViewTemplate.Identifier, project => new SilentRenewCallbackViewTemplate(project));
            RegisterTemplate(AngularUserInfoServiceTemplate.Identifier, project => new AngularUserInfoServiceTemplate(project));
        }
    }
}
