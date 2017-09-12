using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.VisualStudio;
using Intent.SoftwareFactory.VSProjects.Decorators;
using Intent.SoftwareFactory.VSProjects.Templates.VisualStudio2015Solution;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Hosting;
using Intent.SoftwareFactory.MetaData;

namespace Intent.SoftwareFactory.VSProjects.Registrations
{
    public class VSRegistrations : OldProjectTemplateRegistration
    {
        public const string CSharpLibrary = "0FEBBF41-7C8E-4F98-85A5-F8B5236CFD7D";
        public const string WebApiApplication = "8AF747CF-58F0-449C-8B95-46080FEFC8C0";
        public const string WcfApplication = "3CDFF513-03D8-4BAB-9435-160108A086A3";
        public const string ConsoleApplication = "673AAE96-C9B1-4B7E-9A52-ADE5F9218CFC";
        public const string NodeJsConsoleApplication = "CC13FD07-C783-4B0D-A641-4A861A22F087";

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            SolutionFile existingSolution = null;
            if (File.Exists(application.GetSolutionPath()))
            {
                existingSolution = SolutionFile.Parse(application.GetSolutionPath());
            }
            RegisterApplicationTeamplate(new VisualStudio2015SolutionTemplate(application, existingSolution));

            RegisterDecorator<IWebConfigDecorator>(CoreDecoratorId.AssemblyBindingRedirectWebConfig, new AssemblyBindingRedirectWebConfigDecorator());

            foreach (var project in application.Projects)
            {
                if (project.ProjectType.Id == CSharpLibrary)
                {
                    project.RegisterTemplateInstance(new Templates.LibraryCSProjectFile.LibraryCSProjectFileTemplate(project));
                    project.RegisterTemplateInstance(new Templates.NuGetPackagesConfig.NuGetPackagesConfigTemplate(project));
                    project.RegisterTemplateInstance(new Templates.AssemblyInfo.AssemblyInfoTemplate(project));
                }
                else if (project.ProjectType.Id == WebApiApplication)
                {
                    project.RegisterTemplateInstance(new Templates.WebApiServiceCSProjectFile.WebApiServiceCSProjectFileTemplate(project));
                    project.RegisterTemplateInstance(new Templates.WebConfig.WebApiWebConfigFileTemplate(project, application.EventDispatcher));
                    project.RegisterTemplateInstance(new Templates.NuGetPackagesConfig.NuGetPackagesConfigTemplate(project));
                    project.RegisterTemplateInstance(new Templates.AssemblyInfo.AssemblyInfoTemplate(project));
                }
                else if (project.ProjectType.Id == WcfApplication)
                {
                    project.RegisterTemplateInstance(new Templates.WcfServiceCSProjectFile.WcfServiceCSProjectFileTemplate(project));
                    project.RegisterTemplateInstance(new Templates.WebConfig.WebApiWebConfigFileTemplate(project, application.EventDispatcher));
                    project.RegisterTemplateInstance(new Templates.NuGetPackagesConfig.NuGetPackagesConfigTemplate(project));
                    project.RegisterTemplateInstance(new Templates.AssemblyInfo.AssemblyInfoTemplate(project));
                }
                else if (project.ProjectType.Id == ConsoleApplication)
                {
                    project.RegisterTemplateInstance(new Templates.ConsoleApp.ConsoleAppCsProjectFile.ConsoleAppCsProjectFileTemplate(project));
                    project.RegisterTemplateInstance(new Templates.NuGetPackagesConfig.NuGetPackagesConfigTemplate(project));
                    project.RegisterTemplateInstance(new Templates.AssemblyInfo.AssemblyInfoTemplate(project));
                }
                else if (project.ProjectType.Id == NodeJsConsoleApplication)
                {
                    project.RegisterTemplateInstance(new Templates.NodeJSProjectFile.NodeJSProjectFileTemplate(project));
                }
            }

            //RegisterTemplate(CoreTemplateId.ConsoleApp, project => new Templates.ConsoleApp.ConsoleAppTemplate(project));

            //RegisterTemplate(CoreTemplateId.ProjectCSLibrary, project => new Templates.LibraryCSProjectFile.LibraryCSProjectFileTemplate(project));
            //RegisterTemplate(CoreTemplateId.ConsoleAppCsProject, project => new Templates.ConsoleApp.ConsoleAppCsProjectFile.ConsoleAppCsProjectFileTemplate(project));
            //RegisterTemplate(CoreTemplateId.NuGetPackagesConfig, project => new Templates.NuGetPackagesConfig.NuGetPackagesConfigTemplate(project));
            //RegisterTemplate(CoreTemplateId.AssemblyInfo, project => new Templates.AssemblyInfo.AssemblyInfoTemplate(project));
            //RegisterTemplate(CoreTemplateId.ProjectWCF, project => new Templates.WcfServiceCSProjectFile.WcfServiceCSProjectFileTemplate(project));
            //RegisterTemplate(CoreTemplateId.WcfServiceWebConfig, project => new Templates.WebConfig.WcfServiceWebConfigTemplate(project));
            //RegisterTemplate(CoreTemplateId.ProjectWebApi, project => new Templates.WebApiServiceCSProjectFile.WebApiServiceCSProjectFileTemplate(project, hostingConfig));
            //RegisterTemplate(CoreTemplateId.WebApiWebConfig, project => new Templates.WebConfig.WebApiWebConfigFileTemplate(project, application.EventDispatcher));

        }
    }
}
