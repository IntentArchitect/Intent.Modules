using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateTemplate : IntentProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.T4Template";

        public ProjectItemTemplateTemplate(string templateId, IProject project, IClass model) : base(templateId, project, model)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: "Templates\\${Model.Name}");
        }

        public override string TransformText()
        {
            var content = TemplateHelper.GetExistingTemplateContent(this);
            if (content != null)
            {
                return TemplateHelper.ReplaceTemplateInheritsTag(content, $"IntentProjectItemTemplateBase<{GetModelType()}>");
            }

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
            if (Model.GetCreationMode() == CreationMode.SingleFileListModel)
            {
                type = $"IList<{type}>";
            }

            return type;
        }

    }
    
}