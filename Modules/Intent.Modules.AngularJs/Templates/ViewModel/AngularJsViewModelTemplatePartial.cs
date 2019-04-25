using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates

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
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}ViewModel",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\States\{Model.Name}",
                className: "${Model.Name}ViewModel",
                @namespace: "${Project.ApplicationName}");
        }
    }
}
