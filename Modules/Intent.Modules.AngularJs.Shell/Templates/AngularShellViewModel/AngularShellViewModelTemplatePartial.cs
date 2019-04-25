using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.AngularJs.Shell.Templates.AngularShellViewModel
{
    partial class AngularShellViewModelTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.Shell.ShellViewModel";
        public AngularShellViewModelTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "ShellViewModel",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Shell"
                );
        }
    }
}
