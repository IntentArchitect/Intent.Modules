using Intent.Modules.AngularJs.Templates.ViewModel;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
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

        public string Url => Model.GetStereotypeProperty("AngularState", "Url", "");

        public string ViewModelName
        {
            get
            {
                var template = Project.FindTemplateInstance(TemplateDependancy.OnModel(AngularJsViewModelTemplate.Identifier, Model)) as IHasClassDetails;
                if (template != null)
                {
                    return template.ClassName;
                }
                return $"{Model.Name}ViewModel";
            }
        }

        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledTagWeave,
                fileName: "${Model.Name}StateManager",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\States\{Model.Name}",
                className: "${Model.Name}StateManager",
                @namespace: "${Project.ApplicationName}");
        }
    }
}
