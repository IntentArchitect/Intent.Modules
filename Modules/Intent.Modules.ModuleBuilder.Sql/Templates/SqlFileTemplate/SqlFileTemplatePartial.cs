using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.ModuleBuilder.Sql.Api;
using Intent.Modules.Common.CSharp.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Sql.Templates.SqlFileTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class SqlFileTemplate : IntentTemplateBase<SqlTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Sql.Templates.SqlFileTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SqlFileTemplate(IOutputTarget outputTarget, SqlTemplateModel model) : base(TemplateId, outputTarget, model)
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
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.Name)}",
                fileExtension: "tt");
        }

        public override string TransformText()
        {
            var content = GetExistingTemplateContent();
            if (content != null)
            {
                return ReplaceTemplateInheritsTag(content, $"SqlTemplateBase<{Model.GetModelName()}>");
            }

            return $@"<#@ template language=""C#"" inherits=""SqlTemplateBase<{Model.GetModelName()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Sql.Templates"" #>
<#@ import namespace=""Intent.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{(Model.GetModelType() != null ? $@"<#@ import namespace=""{Model.GetModelType()?.ParentModule.ApiNamespace}"" #>" : "")}
{TemplateBody()}";
        }

        private string TemplateBody()
        {
            return @"
-- Create your SQL template here.";
        }



        private string GetExistingTemplateContent()
        {
            var fileLocation = FileMetadata.GetFullLocationPathWithFileName();

            if (File.Exists(fileLocation))
            {
                return File.ReadAllText(fileLocation);
            }

            return null;
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