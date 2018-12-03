using System.IO;
using System.Text;
using Intent.Metadata.Models;
using Intent.MetaModel.DTO;
using Intent.Modules.Common.MetaData;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateTemplate : IntentProjectItemTemplateBase<IAttribute>
    {
        public const string TemplateId = "Intent.ModuleBuilder.RoslynProjectItemTemplate.T4Template";

        public RoslynProjectItemTemplateTemplate(string templateId, IProject project, IAttribute model) : base(templateId, project, model)
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
            return $@"<#@ template language=""C#"" inherits=""IntentRoslynProjectItemTemplateBase<IClass>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""Intent.SoftwareFactory.Templates"" #>
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
    }
    
}