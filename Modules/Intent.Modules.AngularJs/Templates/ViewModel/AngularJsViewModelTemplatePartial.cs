using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Templates.ViewModel
{
    partial class AngularJsViewModelTemplate : IntentTypescriptProjectItemTemplateBase<ViewStateModel>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.ViewModel";
        public AngularJsViewModelTemplate(IProject project, ViewStateModel model)
            : base(Identifier, project, model)
        {
        }

        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                    codeGenType: CodeGenType.Basic,
                    fileName: "${Name}ViewModel",
                    fileExtension: "ts",
                    defaultLocationInProject: $@"wwwroot\App\States\{Model.Name}",
                    className: "${Name}ViewModel",
                    @namespace: "${Project.ApplicationName}");
        }
    }
}
