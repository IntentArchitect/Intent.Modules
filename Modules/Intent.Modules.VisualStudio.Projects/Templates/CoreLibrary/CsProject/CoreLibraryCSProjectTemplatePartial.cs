using System;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    partial class CoreLibraryCSProjectTemplate : IntentProjectItemTemplateBase<IVisualStudioProject>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreLibrary.CSProject";

        public CoreLibraryCSProjectTemplate(IProject project, IVisualStudioProject model)
            : base(Identifier, project, model)
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
