using System.IO;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateTemplate : IntentProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.T4Template";

        public ProjectItemTemplateTemplate(string templateId, IProject project, IClass model) : base(templateId, project, model)
        {
        }

        public string FolderPath => string.Join("\\", new [] { "Templates" }.Concat(Model.GetFolderPath(false).Select(x => x.Name).ToList()));

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: "${FolderPath}/${Model.Name}");
        }

        public override string TransformText()
        {
            return $@"<#@ template language=""C#"" inherits=""IntentProjectItemTemplateBase<{GetModelType()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>

// Place your file template logic here
";
        }

        private string GetModelType()
        {
            var type = Model.GetTargetModel();
            if (Model.GetRegistrationType() == RegistrationType.SingleFileListModel)
            {
                type = $"IList<{type}>";
            }

            return type;
        }

    }

}