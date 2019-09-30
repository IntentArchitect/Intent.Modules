using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Intent.Modules.Bower.Installer;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.SoftwareFactory;
using Intent.Engine;
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
            }
            else if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                if (!AngularInstalled(application))
                {
                    RunAngularCli(application);
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
                configuration.RootPath = ""Client/dist"";
            }});
        }}" }
            });

            application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"Microsoft.AspNetCore.SpaServices.AngularCli;" },
                { InitializationRequiredEvent.CallKey, $@"InitializeAngularSpa(app, env);" },
                { InitializationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void InitializeAngularSpa(IApplicationBuilder app, IHostingEnvironment env)
        {{
            app.UseSpa(spa =>
            {{
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = ""Client"";

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
            var project = application.Projects.FirstOrDefault(x => x.ProjectType.Id == VisualStudioProjectTypeIds.CoreWebApp);
            return project != null && File.Exists(Path.Combine(project.ProjectLocation, "Client", "angular.json"));
        }

        public void RunAngularCli(IApplication application)
        {
            var project = application.Projects.FirstOrDefault(x => x.ProjectType.Id == VisualStudioProjectTypeIds.CoreWebApp);
            if (project == null)
            {
                Logging.Log.Failure("Could not find project to install Angular application.");
                return;
            }

            Logging.Log.Info($"Installing Angular into project: [{ project.Name }]");

            CommandLineProcessor cmd = new CommandLineProcessor();

            var command = $@"ng new {application.Name} --directory Client --minimal --defaults";
            try
            {
                var output = cmd.ExecuteCommand(Path.GetFullPath(project.ProjectLocation),
                    new[]
                    {
                        command,
                    });
                Logging.Log.Info(output);
            }
            catch (Exception e)
            {
                Logging.Log.Failure($@"Failed to execute: ""{command}""
Please ensure that both npm and that the Angular CLI have been installed.
To check that you have the npm client installed, run npm -v in a terminal/console window.
To install the CLI using npm, open a terminal/console window and enter the following command: npm install -g @angular/cli.");
                Logging.Log.Failure(e);
            }

        }
    }
}
