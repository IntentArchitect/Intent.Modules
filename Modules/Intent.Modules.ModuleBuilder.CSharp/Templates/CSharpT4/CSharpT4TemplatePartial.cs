using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using CodeGenType = Intent.Modules.Common.CodeGenType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpT4
{
    [IntentManaged(Mode.Merge)]
    public partial class CSharpT4Template : IntentFileTemplateBase<CSharpTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.CSharp.Templates.CSharpT4Template";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CSharpT4Template(IOutputTarget outputTarget, CSharpTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{TemplateName}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.Name.ToCSharpIdentifier().RemoveSuffix("Template"))}",
                fileExtension: "tt");
        }

        public override string TransformText()
        {
            if (TryGetExistingFileContent(out var content))
            {
                return ReplaceTemplateInheritsTag(content, $"{GetBaseType()}");
            }

            return $@"<#@ template language=""C#"" inherits=""{GetBaseType()}"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Modules.Common.CSharp.Templates"" #>
<#@ import namespace=""Intent.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{TemplateBody()}";
        }

        private string TemplateBody()
        {
            return $@"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace <#= Namespace #>
{{
    public {(IsForInterface() ? "interface" : "class")} <#= ClassName #>
    {{

    }}
}}";
        }

        private bool IsForInterface()
        {
            return Model.Name.RemoveSuffix("Template").EndsWith("Interface");
        }

        private string GetBaseType()
        {
            if (Model.DecoratorContract != null)
            {
                return $"CSharpTemplateBase<{Model.GetModelName()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            return $"CSharpTemplateBase<{Model.GetModelName()}>";
        }

        private static readonly Regex _templateInheritsTagRegex = new Regex(
            @"(?<begin><#@[ ]*template[ ]+[\.a-zA-Z0-9=_\""#<> ]*inherits=\"")(?<type>[a-zA-Z0-9\.,_<> ]+)(?<end>\""[ ]*#>)",
            RegexOptions.Compiled);

        private static string ReplaceTemplateInheritsTag(string templateContent, string inheritType)
        {
            return _templateInheritsTagRegex.Replace(templateContent, $"${{begin}}{inheritType}${{end}}");
        }
    }
}