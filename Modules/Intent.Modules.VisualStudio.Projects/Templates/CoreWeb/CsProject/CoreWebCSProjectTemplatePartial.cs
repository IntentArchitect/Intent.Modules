using System;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    partial class CoreWebCSProjectTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.CSProject";

        public CoreWebCSProjectTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Project.Application.EventDispatcher.Publish(new VisualStudioProjectCreatedEvent(Project.Id, GetMetadata().GetFullLocationPathWithFileName()));
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "csproj",
                defaultLocationInProject: ""
            );
        }

        public override string RunTemplate()
        {
            if (DefineDefaultFileMetadata().OverwriteBehaviour != OverwriteBehaviour.OnceOff)
            {
                // Unless onceOff, then on subsequent SF runs, the SF shows two outputs for the same .csproj file.
                throw new Exception("Template must be configured with OverwriteBehaviour.OnceOff.");
            }

            return TransformText();
        }
    }
}
