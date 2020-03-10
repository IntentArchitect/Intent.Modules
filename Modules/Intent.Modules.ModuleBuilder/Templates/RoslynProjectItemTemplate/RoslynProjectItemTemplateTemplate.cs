using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Helpers;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateTemplate : IntentProjectItemTemplateBase<IFileTemplate>
    {
        public const string TemplateId = "Intent.ModuleBuilder.RoslynProjectItemTemplate.T4Template";

        public RoslynProjectItemTemplateTemplate(string templateId, IProject project, IFileTemplate model) : base(templateId, project, model)
        {
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: "${FolderPath}/${Model.Name}");
        }

        public override string TransformText()
        {
            var content = TemplateHelper.GetExistingTemplateContent(this);
            if (content != null)
            {
                return TemplateHelper.ReplaceTemplateInheritsTag(content, $"IntentRoslynProjectItemTemplateBase<{GetModelType()}>");
            }

            return $@"<#@ template language=""C#"" inherits=""IntentRoslynProjectItemTemplateBase<{GetModelType()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{(Model.GetModelType() != null ? $@"<#@ import namespace=""{Model.GetModelType().Namespace}"" #>" : "")}
{TemplateBody()}";
        }

        private string TemplateBody()
        {
            switch (Model.GetCreationMode())
            {
                case CreationMode.SingleFile:
                    return @"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace <#= Namespace #>
{
    public class <#= ClassName #>
    {

    }
}";
                    break;
                case CreationMode.FilePerModel:
                    return Model.GetModelType().PerModelTemplate;
                //case CreationMode.SingleFileListModel:
                //    return Model.GetModelType().SingleListTemplate;
                case CreationMode.Custom:
                    return string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetModelType()
        {
            return Model.GetTemplateModelName();
        }

    }

}