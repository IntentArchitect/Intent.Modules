using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Helpers;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateTemplate : IntentProjectItemTemplateBase<FileTemplateModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.T4Template";

        public ProjectItemTemplateTemplate(string templateId, IProject project, FileTemplateModel model) : base(templateId, project, model)
        {
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath().Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: $"{Model.Name}");
        }

        public override string TransformText()
        {
            var content = TemplateHelper.GetExistingTemplateContent(this);
            if (content != null)
            {
                return TemplateHelper.ReplaceTemplateInheritsTag(content, $"{GetTemplateBaseClass()}<{GetModelType()}>");
            }

            return $@"<#@ template language=""C#"" inherits=""{GetTemplateBaseClass()}<{GetModelType()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{(Model.GetModelType() != null ? $@"<#@ import namespace=""{Model.GetModelType()?.ParentModule.ApiNamespace}"" #>" : "")}

// Place your file template logic here
";
        }

        private string GetTemplateBaseClass()
        {
            return nameof(IntentTemplateBase);
        }

        private string GetModelType()
        {
            return Model.GetModelName();
        }

    }

}