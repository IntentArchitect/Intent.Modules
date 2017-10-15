using System.Collections.Generic;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AngularJs.Templates.View
{
    partial class AngularJsViewTemplate : IntentProjectItemTemplateBase<ViewStateModel>, ITemplate
    {
        public const string Identifier = "Intent.AngularJs.View";
        public AngularJsViewTemplate(IProject project, ViewStateModel model)
            : base(Identifier, project, model)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Name}View",
                fileExtension: "html",
                defaultLocationInProject: $@"wwwroot\App\States\{Model.Name}"
                );
        }
    }
}
