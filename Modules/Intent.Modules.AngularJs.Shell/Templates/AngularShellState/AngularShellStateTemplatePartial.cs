using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.AngularJs.Shell.Templates.AngularShellState
{
    partial class AngularShellStateTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Shell.ShellState";

        public AngularShellStateTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
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
