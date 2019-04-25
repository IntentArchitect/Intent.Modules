using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.VisualStudio.Projects
{
    public static class VisualStudioProjectExtensions
    {
        public static string SolutionFolder(this IProject project)
        {
            return project.Folder.Name;
        }

        public static string TargetFrameworkVersion(this IProject project)
        {
            var targetFramework = project.ProjectType.Properties.FirstOrDefault(x => x.Name == "TargetFramework");
            return project.GetStereotypeProperty("C# .NET", "FrameworkVersion", targetFramework != null ? $"v{targetFramework.Value}" : "v4.5.2");
        }
    }

    //public static class ApplicationStructureExtensions
    //{
    //    public static ProjectConfig AddConsoleAppProject(this ApplicationStructure item, string name)
    //    {
    //        var projectConfig = new ProjectConfig(ProjectType.CSharpProject, name)
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.ConsoleAppCsProject))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.AssemblyInfo))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.NuGetPackagesConfig))

    //            // TODO: We're probably going to need to generalize this template a bit more
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.WebApiWebConfig).WithFileName("App"))
    //            ;
    //        item.Projects.Add(projectConfig);
    //        return projectConfig;
    //    }

    //    public static ProjectConfig AddCSharpLibary(this ApplicationStructure item, string name)
    //    {
    //        var projectConfig = new ProjectConfig(ProjectType.CSharpProject, name)
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.ProjectCSLibrary))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.AssemblyInfo))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.NuGetPackagesConfig))
    //            ;
    //        item.Projects.Add(projectConfig);
    //        return projectConfig;
    //    }

    //    public static ProjectConfig AddEmptyCSharpLibary(this ApplicationStructure item, string name)
    //    {
    //        var projectConfig = new ProjectConfig(ProjectType.CSharpProject, name);
    //        item.Projects.Add(projectConfig);
    //        return projectConfig;
    //    }

    //    public static ProjectConfig AddWebApiProject(this ApplicationStructure item, string name)
    //    {
    //        var projectConfig = new ProjectConfig(ProjectType.CSharpProject, name)
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.ProjectWebApi))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.AssemblyInfo))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.NuGetPackagesConfig))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.WebApiWebConfig))
    //            ;
    //        item.Decorators.Add(CoreDecoratorId.AssemblyBindingRedirectWebConfig);
    //        item.Projects.Add(projectConfig);
    //        return projectConfig;
    //    }

    //    public static ProjectConfig AddWcfProject(this ApplicationStructure item, string name)
    //    {
    //        var projectConfig = new ProjectConfig(ProjectType.CSharpProject, name)
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.ProjectWCF))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.AssemblyInfo))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.NuGetPackagesConfig))
    //            .AddOutput(TemplateOutputConfig.Create(CoreTemplateId.WcfServiceWebConfig))
    //            ;
    //        item.Decorators.Add(CoreDecoratorId.AssemblyBindingRedirectWebConfig);
    //        item.Projects.Add(projectConfig);
    //        return projectConfig;
    //    }

    //    public static ProjectConfig AddNodeJsProject(this ApplicationStructure item, string name)
    //    {
    //        var projectConfig = new ProjectConfig(ProjectType.NodeJsProject, name)
    //            ;
    //        item.Projects.Add(projectConfig);
    //        return projectConfig;
    //    }
    //}
}