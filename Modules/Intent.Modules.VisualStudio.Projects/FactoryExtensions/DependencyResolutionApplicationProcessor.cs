using Intent.Modules.Common.Plugins;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    [Description("Visual Studio Dependancy Resolver")]
    public class DependencyResolutionFactoryExtensions : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id
        {
            get
            {
                return "Intent.VSProjects.DependencyResolver";
            }
        }

        public override int Order => 0;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                ResolveNuGetDependencies(application);
            }
            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                ResolveProjectDependencies(application);
            }
        }

        public void ResolveNuGetDependencies(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info($"Resolving NuGet Dependencies");

            foreach (var project in application.Projects)
            {
                project.InitializeVSMetadata();
                 
                project.AddNugetPackages(GetTemplateNugetDependencies(project));

                var assemblyDependencies = project.TemplateInstances
                        .SelectMany(ti => ti.GetAllAssemblyDependencies())
                        .Distinct()
                        .ToList();

                foreach (var assemblyDependency in assemblyDependencies)
                {
                    project.AddReference(assemblyDependency);
                }
            }
        }


        public void ResolveProjectDependencies(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info($"Resolving Project Dependencies");

            foreach (var project in application.Projects)
            {
                // 1. Identify project dependencies (for debugging purposes - does nothing):
                //var p = project.TemplateInstances.Select(x => new { x, deps = x.GetAllTemplateDependencies().ToList() }).Where(x => x.deps.Any()).ToList();

                var nullDependencies = project.TemplateInstances.Where(x => x.GetAllTemplateDependencies().Any(d => d == null)).ToList();
                if (nullDependencies.Any())
                {
                    var templates = nullDependencies.First().Id;
                    throw new Exception($"The following template is returning a 'null' template dependency [{templates}]. Please check your GetTemplateDependencies() method.");
                }

                var templateDependencies = project.TemplateInstances
                        .SelectMany(ti => ti.GetAllTemplateDependencies())
                        .Distinct()
                        .ToList();

                var projectDependencies =
                    templateDependencies.Select(x => project.Application.FindOutputTargetWithTemplateInstance(x))
                        .Where(x => x != null && !x.Equals(project))
                        .Distinct()
                        .ToList();

                foreach (var projectDependency in projectDependencies)
                {
                    // 2. Add project dependencies
                    project.AddDependency(projectDependency);
                }
            }
        }

        private IEnumerable<INugetPackageInfo> GetTemplateNugetDependencies(IOutputTarget project)
        {
            return project.TemplateInstances
                    .SelectMany(ti => ti.GetAllNugetDependencies())
                    .Distinct()
                    .ToList();
        }
    }
}

