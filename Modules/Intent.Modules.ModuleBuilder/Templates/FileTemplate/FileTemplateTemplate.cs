using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Helpers;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.FileTemplate
{
    public class FileTemplateTemplate : IntentFileTemplateBase<FileTemplateModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.T4Template";

        public FileTemplateTemplate(string templateId, IOutputTarget project, FileTemplateModel model) : base(templateId, project, model)
        {
        }


        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        public IList<string> OutputFolders => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolders);

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{TemplateName}",
                fileExtension: "tt",
                relativeLocation: $"{FolderPath}");
        }


        public override string TransformText()
        {
            var content = TemplateHelper.GetExistingTemplateContent(this);
            if (content != null)
            {
                return TemplateHelper.ReplaceTemplateInheritsTag(content, $"{GetBaseType()}");
            }

            return $@"<#@ template language=""C#"" inherits=""{GetBaseType()}"" #>
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

        private string GetBaseType()
        {
            if (Model.DecoratorContract != null)
            {
                return $"{GetTemplateBaseClass()}<{Model.GetModelName()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            return $"{GetTemplateBaseClass()}<{Model.GetModelName()}>";
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