using Intent.MetaModel.Hosting;
using Intent.Modules.Bower.Contracts;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.AuthModule
{
    partial class AngularImplicitAuthModuleTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasBowerDependencies, IRequiresPreProcessing, IHasDecorators<IAngularImplicitAuthModuleDecorator>
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.AuthModule";

        private readonly HostingConfigModel _hostingConfig;
        private readonly IApplicationEventDispatcher _applicationEvents;
        private readonly ISolutionEventDispatcher _solutionEvents;
        private readonly DecoratorDispatcher<IAngularImplicitAuthModuleDecorator> _decoratorDispatcher;

        public AngularImplicitAuthModuleTemplate(IProject project, HostingConfigModel hostingConfig, IApplicationEventDispatcher applicationEvents, ISolutionEventDispatcher solutionEvents)
            : base(Identifier, project, null)
        {
            _decoratorDispatcher = new DecoratorDispatcher<IAngularImplicitAuthModuleDecorator>(project.ResolveDecorators<IAngularImplicitAuthModuleDecorator>);
            _hostingConfig = hostingConfig ?? new HostingConfigModel() { UseSsl = true, SslPort = "44399" };
            _applicationEvents = applicationEvents;
            _solutionEvents = solutionEvents;
            _solutionEvents.Subscribe(SolutionEvents.ResourceAvailable_IdentityServer, HandleIdentityServerAvailable);
        }

        public string BasePathConfigKey => $"{Project.Application.SolutionName}_{Project.ApplicationName()}_basepath".ToLower();
        public string ApplicationName => Project.ApplicationName();

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "ImplicitAuthModule",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Auth"
                );
        }

        public string AdditionalIfElses()
        {
            return _decoratorDispatcher.Dispatch(x => x.AdditionalElseIf());
        }

        public IEnumerable<IBowerPackageInfo> GetBowerDependencies()
        {
            return new[]
            {
                BowerPackages.OidcTokenManager,
            };
        }

        public void PreProcess()
        {
            _applicationEvents.Publish(ApplicationEvents.AngularJs_ModuleRegistered, new Dictionary<string, string>()
            {
                { "ModuleName", "Auth" }
            });
            _applicationEvents.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                {"Key", BasePathConfigKey},
                {"Value", _hostingConfig.GetBaseUrl()},
            });

            _solutionEvents.Publish(SolutionEvents.Authentication_ClientRequired, new Dictionary<string, string>()
            {
                { "AuthenticationType", "Implicit"},
                { "ApplicationName", Project.ApplicationName() },
                { "ApplicationUrl", _hostingConfig.GetBaseUrl() },
            });
        }

        private void HandleIdentityServerAvailable(SolutionEvent @event)
        {
            _applicationEvents.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                {"Key", "identity_authority_url"},
                {"Value", @event.GetValue("AuthorityUrl") },
            });

            // TODO: This is dubious
            _decoratorDispatcher.Dispatch(x => x.OnIdentityServerAvailable(@event.GetValue("BaseUrl"), @event.GetValue("AuthorityUrl")));
        }

        public IEnumerable<IAngularImplicitAuthModuleDecorator> GetDecorators()
        {
            return _decoratorDispatcher.GetDecorators();
        }
    }
}
