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
    public class InMemoryContainerRegistration : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override int Order => 0;
        public override string Id => "Intent.EntityFrameworkCore.Extensions.UseInMemory";

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                var dbContextTemplate = application.FindTemplateInstance<DbContextTemplate>(DbContextTemplate.Identifier);

                application.EventDispatcher.Publish(ContainerRegistrationForDbContextEvent.EventId, new Dictionary<string, string>()
                {
                    { ContainerRegistrationForDbContextEvent.UsingsKey, $"Microsoft.EntityFrameworkCore;" },
                    { ContainerRegistrationForDbContextEvent.ConcreteTypeKey, $"{dbContextTemplate.Namespace}.{dbContextTemplate.ClassName}" },
                    { ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey, DbContextTemplate.Identifier },
                    { ContainerRegistrationForDbContextEvent.OptionsKey, $@".UseInMemoryDatabase(""{application.Name}DB"")" },
                    { ContainerRegistrationForDbContextEvent.NugetDependency, NugetPackages.EntityFrameworkCoreInMemory.Name },
                    { ContainerRegistrationForDbContextEvent.NugetDependencyVersion, NugetPackages.EntityFrameworkCoreInMemory.Version },
                });
            }
        }

    }
}
