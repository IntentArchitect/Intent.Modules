using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.NuGetPackagesConfig
{
    public partial class NuGetPackagesConfigTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.NuGetPackagesConfig";

        public NuGetPackagesConfigTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "packages",
                fileExtension: "config",
                defaultLocationInProject: "");
        }
    }
}
