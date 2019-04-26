using System.Collections.Generic;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Docker.Templates.DockerFile
{
    partial class DockerfileTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.AspNetCore.Dockerfile";


        public DockerfileTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }


        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftVisualStudioAzureContainersToolsTargets
            };
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "Dockerfile",
                fileExtension: "",
                defaultLocationInProject: ""
                );
        }

        public void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(LaunchProfileRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { LaunchProfileRegistrationEvent.ProfileNameKey, "Docker" },
                { LaunchProfileRegistrationEvent.CommandNameKey, "Docker" },
                { LaunchProfileRegistrationEvent.LaunchBrowserKey, "true" },
                { LaunchProfileRegistrationEvent.LaunchUrlKey, "{Scheme}://localhost:{ServicePort}" },
            });
        }
    }
}
