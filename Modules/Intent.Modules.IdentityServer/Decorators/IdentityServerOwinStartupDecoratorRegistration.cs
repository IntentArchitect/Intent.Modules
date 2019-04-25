using Intent.MetaModel.Hosting;
using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using System;
using System.Linq;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.IdentityServer.Decorators
{
    public class IdentityServerOwinStartupDecoratorRegistration : DecoratorRegistration<IOwinStartupDecorator>
    {
        public override string DecoratorId => IdentityServerOwinStartupDecorator.Identifier;

        private readonly IMetadataManager _metaDataManager;

        public IdentityServerOwinStartupDecoratorRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override object CreateDecoratorInstance(IApplication application)
        {
            var hostingConfig = _metaDataManager.GetMetaData<HostingConfigModel>("LocalHosting").SingleOrDefault(x => x.ApplicationName == application.ApplicationName);
            return new IdentityServerOwinStartupDecorator(hostingConfig, application.EventDispatcher, application.SolutionEventDispatcher);
        }
    }
}
