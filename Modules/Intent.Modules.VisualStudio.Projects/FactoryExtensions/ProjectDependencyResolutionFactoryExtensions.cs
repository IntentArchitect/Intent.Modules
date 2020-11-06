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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    [Description("Visual Studio Dependancy Resolver")]
    public class ProjectDependencyResolutionFactoryExtensions : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id
        {
            get
            {
                return "Intent.VSProjects.ProjectDependencyResolver";
            }
        }

        public override int Order => 0;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                ResolveProjectDependencies(application);
            }
        }

        public void ResolveProjectDependencies(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info($"Resolving Project Dependencies");

            foreach (var outputTarget in application.OutputTargets)
            {
                // 1. Identify project dependencies (for debugging purposes - does nothing):
                //var p = project.TemplateInstances.Select(x => new { x, deps = x.GetAllTemplateDependencies().ToList() }).Where(x => x.deps.Any()).ToList();
                var project = outputTarget.GetTargetPath()[0]; // root is the project itself

                var nullDependencies = outputTarget.TemplateInstances.Where(x => x.GetAllTemplateDependencies().Any(d => d == null)).ToList();
                if (nullDependencies.Any())
                {
                    var templates = nullDependencies.First().Id;
                    throw new Exception($"The following template is returning a 'null' template dependency [{templates}]. Please check your GetTemplateDependencies() method.");
                }

                var templateDependencies = outputTarget.TemplateInstances
                        .SelectMany(ti => ti.GetAllTemplateDependencies())
                        .Distinct()
                        .ToList();

                var projectDependencies =
                    templateDependencies.Select(x => outputTarget.Application.FindOutputTargetWithTemplate(x)?.GetTargetPath()[0])
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

