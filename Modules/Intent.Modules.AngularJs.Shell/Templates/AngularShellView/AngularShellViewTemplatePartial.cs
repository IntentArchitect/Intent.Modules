using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AngularJs.Shell.Templates.AngularShellView
{
    partial class AngularShellViewTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasBowerDependencies
    {
        public const string Identifier = "Intent.AngularJs.Shell.ShellView";
        public AngularShellViewTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "ShellView",
                fileExtension: "html",
                defaultLocationInProject: @"wwwroot\App\Shell"
                );
        }

        public IEnumerable<IBowerPackageInfo> GetBowerDependencies()
        {
            return new[]
            {
                BowerPackageInfo.FontAwesome
            };
        }
    }
}
