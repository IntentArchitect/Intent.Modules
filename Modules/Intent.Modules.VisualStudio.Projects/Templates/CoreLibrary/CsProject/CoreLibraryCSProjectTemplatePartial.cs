using System;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    partial class CoreLibraryCSProjectTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IProjectTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreLibrary.CSProject";

        public CoreLibraryCSProjectTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "csproj",
                defaultLocationInProject: ""
            );
        }

        public override string RunTemplate()
        {
            if (DefineDefaultFileMetaData().OverwriteBehaviour != OverwriteBehaviour.OnceOff)
            {
                // Unless onceOff, then on subsequent SF runs, the SF shows two outputs for the same .csproj file.
                throw new Exception("Template must be configured with OverwriteBehaviour.OnceOff.");
            }

            return TransformText();
        }
    }
}
