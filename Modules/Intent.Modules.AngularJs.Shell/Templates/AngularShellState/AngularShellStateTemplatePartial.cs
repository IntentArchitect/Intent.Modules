using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Shell.Templates.AngularShellState
{
    partial class AngularShellStateTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Shell.ShellState";

        public AngularShellStateTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "ShellStateManager",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Shell"
                );
        }
    }
}
