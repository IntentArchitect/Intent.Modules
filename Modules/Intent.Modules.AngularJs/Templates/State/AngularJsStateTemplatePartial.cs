using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Templates.State
{
    partial class AngularJsStateTemplate : IntentTypescriptProjectItemTemplateBase<ViewStateModel>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.State";

        public AngularJsStateTemplate(IProject project, ViewStateModel model)
            : base(Identifier, project, model)
        {
        }

        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "${Name}State",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\States\{Model.Name}",
                className: "${Name}State",
                @namespace: "${Project.ApplicationName}");
        }
    }
}
