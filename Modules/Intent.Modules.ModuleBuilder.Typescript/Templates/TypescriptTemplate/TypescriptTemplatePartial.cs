using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Typescript.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Modules.ModuleBuilder.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Typescript.Templates.TypescriptTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TypescriptTemplate : IntentProjectItemTemplateBase<TypescriptFileTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Typescript.Templates.TypescriptTemplate";

        public TypescriptTemplate(IProject project, TypescriptFileTemplateModel model) : base(TemplateId, project, model)
        {
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "tt",
                defaultLocationInProject: "${FolderPath}/${Model.Name}");
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();

        public string FolderPath => string.Join("/", FolderBaseList);

        public override string TransformText()
        {
            var content = GetExistingTemplateContent();
            if (content != null)
            {
                return ReplaceTemplateInheritsTag(content, $"TypeScriptTemplateBase<{Model.GetModelName()}>");
            }

            return $@"<#@ template language=""C#"" inherits=""TypeScriptTemplateBase<{Model.GetModelName()}>"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.TypeScript.Templates"" #>
<#@ import namespace=""Intent.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{(Model.GetModeler() != null ? $@"<#@ import namespace=""{Model.GetModeler().ApiNamespace}"" #>" : "")}
{TemplateBody()}";
        }

        private string TemplateBody()
        {
            return @"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace <#= Namespace #>
{
    public class <#= ClassName #>
    {

    }
}";
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