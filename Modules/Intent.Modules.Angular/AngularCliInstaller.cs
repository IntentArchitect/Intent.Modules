using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Angular
{
    [Description("Angular CLI Installer")]
    public class AngularCliInstaller : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override int Order => 100;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                RequestInitialization(application);
                //}
                //else if (step == ExecutionLifeCycleSteps.BeforeCommitChanges )
                //{
                if (!AngularInstalled(application))
                {
                    var outputTarget = CliCommand.GetWebCoreProject(application);
                    if (outputTarget == null)
                    {
                        Logging.Log.Failure("Could not find project to install Angular application.");
                        return;
                    }
                    Logging.Log.Info($"Installing Angular into project: [{ outputTarget.Name }]");
                    CliCommand.Run(outputTarget.Location, $@"ng new {application.Name} --directory ClientApp --minimal --defaults --skipGit=true --force=true");
                    CliCommand.Run(outputTarget.Location, $@"npm i @types/node@8.10.52"); // Ensure this version - typescript fix
                }
                else
                {
                    Logging.Log.Info("Angular app already installed. Skipping Angular CLI installation");
                }
            }
        }

        public override string Id => "Intent.Angular.CLIInstaller";

        private void RequestInitialization(IApplication application)
        {
            application.EventDispatcher.Publish(ServiceConfigurationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { ServiceConfigurationRequiredEvent.UsingsKey, $@"Microsoft.AspNetCore.SpaServices.AngularCli;" },
                { ServiceConfigurationRequiredEvent.CallKey, "ConfigureAngularSpa(services);" },
                { ServiceConfigurationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void ConfigureAngularSpa(IServiceCollection services)
        {{
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {{
                configuration.RootPath = ""ClientApp/dist"";
            }});
        }}" }
            });

            application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"Microsoft.AspNetCore.SpaServices.AngularCli;" },
                { InitializationRequiredEvent.CallKey, $@"InitializeAngularSpa(app, env);" },
                { InitializationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void InitializeAngularSpa(IApplicationBuilder app, { (CliCommand.GetWebCoreProject(application).IsNetCore2App() ? "IHostingEnvironment" : "IWebHostEnvironment") } env)
        {{
            app.UseSpa(spa =>
            {{
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = ""ClientApp"";

                if (env.IsDevelopment())
                {{
                    spa.UseAngularCliServer(npmScript: ""start"");
                }}
            }});
        }}" }
            });
        }

        public bool AngularInstalled(IApplication application)
        {
            var project = CliCommand.GetWebCoreProject(application);
            return project != null && File.Exists(Path.Combine(project.Location, "ClientApp", "angular.json"));
        }
    }
}
