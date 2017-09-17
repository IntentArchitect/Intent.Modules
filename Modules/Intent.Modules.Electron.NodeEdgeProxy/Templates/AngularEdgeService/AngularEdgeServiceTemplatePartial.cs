using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularEdgeService
{
    partial class AngularEdgeServiceTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {

        public const string Identifier = "Intent.Electron.NodeEdgeProxy.AngularEdgeService";

        public AngularEdgeServiceTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "Edge",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Edge\Services"
                );
        }
    }
}
