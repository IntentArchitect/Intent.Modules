using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Plugins;
using Intent.Plugins.FactoryExtensions;

namespace Intent.Modules.AspNetCore.Swashbuckle.FactoryExtensions
{
    public class SwashbuckleSetupFactoryExtension : IFactoryExtension, IExecutionLifeCycle
    {
        public string Id => "Intent.AspNetCore.Swashbuckle.Setup";

        public int Order => 0;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                RequestInitialization(application);
            }
        }

        private void RequestInitialization(IApplication application)
        {
            application.EventDispatcher.Publish(ServiceConfigurationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { ServiceConfigurationRequiredEvent.UsingsKey, $@"Swashbuckle.AspNetCore.Swagger;" },
                { ServiceConfigurationRequiredEvent.CallKey, "ConfigureSwagger(services);" },
                { ServiceConfigurationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void ConfigureSwagger(IServiceCollection services)
        {{
            services.AddSwaggerGen(c =>
            {{
                c.SwaggerDoc(""v1"", new Info {{ Title = ""{application.ApplicationName} API"", Version = ""v1"" }});
            }});
        }}" }
            });

            application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"Swashbuckle.AspNetCore.Swagger;" },
                { InitializationRequiredEvent.CallKey, $@"InitializeSwagger(app);" },
                { InitializationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void InitializeSwagger(IApplicationBuilder app)
        {{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {{
                c.SwaggerEndpoint(""/swagger/v1/swagger.json"", ""{application.ApplicationName} API V1"");
            }});
        }}" }
            });
        }
    }
}
