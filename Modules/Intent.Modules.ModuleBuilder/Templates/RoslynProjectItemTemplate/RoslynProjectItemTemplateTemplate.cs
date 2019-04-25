using System.IO;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.MetaModel.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateTemplate : IntentProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.RoslynProjectItemTemplate.T4Template";

        public RoslynProjectItemTemplateTemplate(string templateId, IProject project, IClass model) : base(templateId, project, model)
        {
        }

        public string FolderPath => string.Join("\\", new [] { "Templates" }.Concat(Model.GetFolderPath(false).Select(x => x.Name).ToList()));

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: "${FolderPath}\\${Model.Name}");
        }

        public override string TransformText()
        {
            return $@"<#@ template language=""C#"" inherits=""IntentRoslynProjectItemTemplateBase<{GetModelType()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>

using System;
<#=DependencyUsings#>
// Mode.Fully will overwrite file on each run. 
// Add in explicit [IntentManaged.Ignore] attributes to class or methods. Alternatively change to Mode.Merge (additive) or Mode.Ignore (once-off)
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace <#= Namespace #>
{{
    public class <#= ClassName #>
    {{{TemplateBody()}
    }}
}}";
        }

        private string TemplateBody()
        {
            if (Model.GetRegistrationType() == RegistrationType.FilePerModel)
            {
                return @"
<#  // The following is an example template implementation
    foreach(var attribute in Model.Attributes) { #>
        public <#= Types.Get(attribute.Type) #> <#= attribute.Name.ToPascalCase() #> { get; set; }

<#  } #>

<#  foreach(var operation in Model.Operations) { #>
        public <#= operation.ReturnType != null ? Types.Get(operation.ReturnType.Type) : ""void"" #> <#= operation.Name.ToPascalCase() #>(<#= string.Join("", "", operation.Parameters.Select(x => string.Format(""{0} {1}"", Types.Get(x.Type), x.Name))) #>)
        {
            throw new NotImplementedException();
        }

<#  } #>";
            }

            if (Model.GetRegistrationType() == RegistrationType.SingleFileListModel)
            {
                return @"
<#  // The following is an example template implementation
    foreach(var model in Model) { #>
        // Model found: <#= model.Name #>
<#  } #>";
            }

            return string.Empty;
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