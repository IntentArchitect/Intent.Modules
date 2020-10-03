using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    public class SqlServerContainerRegistration : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override int Order => 0;
        public override string Id => "Intent.EntityFrameworkCore.Extensions.UseSqlServer";

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                application.EventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
                {
                    { "Name", $"{application.Name}DB" },
                    { "ConnectionString", $"Server=.;Initial Catalog={application.SolutionName}.{ application.Name };Integrated Security=true;MultipleActiveResultSets=True" },
                    { "ProviderName", "System.Data.SqlClient" },
                });

                var dbContextTemplate = application.FindTemplateInstance<DbContextTemplate>(DbContextTemplate.Identifier);

                application.EventDispatcher.Publish(ContainerRegistrationForDbContextEvent.EventId, new Dictionary<string, string>()
                {
                    { ContainerRegistrationForDbContextEvent.UsingsKey, $"Microsoft.EntityFrameworkCore;" },
                    { ContainerRegistrationForDbContextEvent.ConcreteTypeKey, $"{dbContextTemplate.Namespace}.{dbContextTemplate.ClassName}" },
                    { ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey, DbContextTemplate.Identifier },
                    { ContainerRegistrationForDbContextEvent.OptionsKey, $@".UseSqlServer(Configuration.GetConnectionString(""{application.Name}DB"")){(dbContextTemplate.UseLazyLoadingProxies ? ".UseLazyLoadingProxies()" : "")}" },
                    { ContainerRegistrationForDbContextEvent.NugetDependency, NugetPackages.EntityFrameworkCoreSqlServer.Name },
                    { ContainerRegistrationForDbContextEvent.NugetDependencyVersion, NugetPackages.EntityFrameworkCoreSqlServer.Version },
                });
            }
        }

    }
}
