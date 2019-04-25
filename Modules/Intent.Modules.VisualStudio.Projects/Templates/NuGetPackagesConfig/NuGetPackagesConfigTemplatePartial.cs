using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.VisualStudio.Projects.Templates.NuGetPackagesConfig
{
    public partial class NuGetPackagesConfigTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.NuGetPackagesConfig";

        public NuGetPackagesConfigTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "packages",
                fileExtension: "config",
                defaultLocationInProject: "");
        }
    }
}
