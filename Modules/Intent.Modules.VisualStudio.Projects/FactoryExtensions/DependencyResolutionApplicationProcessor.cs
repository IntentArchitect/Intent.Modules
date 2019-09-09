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


        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                ResolveDependencies(application);
            }
        }

        public void ResolveDependencies(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info($"Resolving Dependencies");


            foreach (var project in application.Projects)
            {
                project.InitializeVSMetadata();
                // 1. Identify project dependencies.
                var p = project.TemplateInstances.Select(x => new { x, deps = x.GetAllTemplateDependencies().ToList() }).Where(x => x.deps.Any()).ToList();

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
                    templateDependencies.Select(x => project.Application.FindProjectWithTemplateInstance(x))
                        .Where(x => x != null && !x.Equals(project))
                        .Distinct()
                        .ToList();

                foreach (var projectDependency in projectDependencies)
                {
                    // 2. Add project dependencies
                    project.AddDependency(projectDependency);

                    // 3. Load nuget packages from project dependencies.
                    //    Don't load for Core projects
                    if (new[] { VisualStudioProjectTypeIds.CoreWebApp, VisualStudioProjectTypeIds.CoreCSharpLibrary }.Contains(project.ProjectType.Id))
                    {
                        continue;
                    }
                    project.AddNugetPackages(GetTemplateNugetDependencies(projectDependency));
                }

                // 4. Load this project's nuget dependencies.
                project.AddNugetPackages(GetTemplateNugetDependencies(project));

                // 5. Load reference assembly dependencies.            
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

        private IEnumerable<INugetPackageInfo> GetTemplateNugetDependencies(IProject project)
        {
            return project.TemplateInstances
                    .SelectMany(ti => ti.GetAllNugetDependencies())
                    .Distinct()
                    .ToList();
        }
    }
}

