using System.IO;
using System.Text;
using Intent.Metadata.Models;
using Intent.MetaModel.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateTemplate : IntentProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.RoslynProjectItemTemplate.T4Template";

        public RoslynProjectItemTemplateTemplate(string templateId, IProject project, IClass model) : base(templateId, project, model)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: "Templates\\${Model.Name}");
        }

        public override string TransformText()
        {
            return $@"<#@ template language=""C#"" inherits=""IntentRoslynProjectItemTemplateBase<{GetModelType()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>

using System;
<#=DependencyUsings#>

namespace <#= Namespace #>
{{
    public class <#= ClassName #>
    {{
        
    }}
}}";
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