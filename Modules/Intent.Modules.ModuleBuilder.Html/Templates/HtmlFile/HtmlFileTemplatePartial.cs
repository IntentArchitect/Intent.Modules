using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Html.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using CodeGenType = Intent.Modules.Common.CodeGenType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Html.Templates.HtmlFile
{
    [IntentManaged(Mode.Merge)]
    partial class HtmlFileTemplate : IntentFileTemplateBase<HtmlFileTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Html.Templates.HtmlFileTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public HtmlFileTemplate(IOutputTarget outputTarget, HtmlFileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: TemplateName,
                fileExtension: "tt",
                relativeLocation: $"{FolderPath}");
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name.ToCSharpIdentifier().RemoveSuffix("Template") }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            if (TryGetExistingFileContent(out var content))
            {
                return ReplaceTemplateInheritsTag(content, $"HtmlTemplateBase<{Model.GetModelName()}>");
            }

            return $@"<#@ template language=""C#"" inherits=""HtmlTemplateBase<{Model.GetModelName()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Modules.Common.Html.Templates"" #>
<#@ import namespace=""Intent.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{(Model.GetModelType() != null ? $@"<#@ import namespace=""{Model.GetModelType()?.ParentModule.ApiNamespace}"" #>" : "")}
{TemplateBody()}";
        }

        private string TemplateBody()
        {
            return @"
<!-- Replace this with your HTML template -->";
        }

        private static readonly Regex _templateInheritsTagRegex = new Regex(
            @"(?<begin><#@[ ]*template[ ]+[\.a-zA-Z0-9=_\""#<> ]*inherits=\"")(?<type>[a-zA-Z0-9\._<>]+)(?<end>\""[ ]*#>)",
            RegexOptions.Compiled);

        private static string ReplaceTemplateInheritsTag(string templateContent, string inheritType)
        {
            return _templateInheritsTagRegex.Replace(templateContent, $"${{begin}}{inheritType}${{end}}");
        }
    }
}