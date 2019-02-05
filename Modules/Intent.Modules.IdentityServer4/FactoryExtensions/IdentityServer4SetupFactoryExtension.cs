using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using System;
using System.Collections.Generic;

namespace Intent.Modules.IdentityServer4.FactoryExtensions
{
    public class IdentityServer4SetupFactoryExtension : IFactoryExtension, IExecutionLifeCycle
    {
        public string Id => "Intent.IdentityServer4.SetupFactoryExtension";

        public int Order => 1;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                SetupAddIdentityServer(application);
            }
        }

        private void SetupAddIdentityServer(IApplication application)
        {
            var serviceConfig = $@"var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            if (Environment.IsDevelopment())
            {{
                builder.AddDeveloperSigningCredential();
            }}
            else
            {{
                throw new Exception(""need to configure key material"");
            }}";

            application.EventDispatcher.Publish(ServiceConfigurationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { ServiceConfigurationRequiredEvent.CallKey, serviceConfig }
            });
        }
    }
}
