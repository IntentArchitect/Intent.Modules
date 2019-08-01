using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial;
using Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Intent.Modules.ModuleBuilder.Helpers
{
    public static class TemplateHelper
    {
        public static string GetExistingTemplateContent<T>(IntentProjectItemTemplateBase<T> template)
        {
            var fileLocation = template.FileMetaData.GetFullLocationPathWithFileName();

            if (File.Exists(fileLocation))
            {
                return File.ReadAllText(fileLocation);
            }

            return null;
        }

        private static readonly Regex _templateInheritsTagRegex = new Regex(
            @"(?<begin><#@[ ]*template[ ]+[\.a-zA-Z0-9=_\""#<> ]*inherits=\"")(?<type>[a-zA-Z0-9\._<>]+)(?<end>\""[ ]*#>)", 
            RegexOptions.Compiled);
        public static string ReplaceTemplateInheritsTag(string templateContent, string inheritType)
        {
            return _templateInheritsTagRegex.Replace(templateContent, $"${{begin}}{inheritType}${{end}}");
        }

        public static IReadOnlyCollection<TemplateDependencyInfo> GetTemplateDependencies(IntentProjectItemTemplateBase<IClass> template, IClass classRepresentingTemplate, IEnumerable<IClass> otherTemplateClasses)
        {
            var infos = GetTemplateDependencyNames(classRepresentingTemplate)
                .Where(p => !string.IsNullOrEmpty(p))
                .SelectMany(s => otherTemplateClasses.Where(p => p.Name == s))
                .Select(s =>
                {
                    if (s.IsCSharpTemplate())
                    {
                        var templateInstance = template.Project.FindTemplateInstance<RoslynProjectItemTemplatePartialTemplate>(RoslynProjectItemTemplatePartialTemplate.TemplateId, s);
                        return new TemplateDependencyInfo(s.Name, templateInstance.GetTemplateId(), templateInstance.Model.GetTargetModel(), "IHasClassDetails");
                    }
                    else if (s.IsFileTemplate())
                    {
                        var templateInstance = template.Project.FindTemplateInstance<ProjectItemTemplatePartialTemplate>(ProjectItemTemplatePartialTemplate.TemplateId, s);
                        return new TemplateDependencyInfo(s.Name, templateInstance.GetTemplateId(), templateInstance.Model.GetTargetModel(), $"IntentProjectItemTemplateBase<{templateInstance.Model.GetTargetModel()}>");
                    }
                    return null;
                })
                .Where(p => p != null);
            var customOne = GetTemplateDependencyNames(classRepresentingTemplate)
                .Where(p => string.IsNullOrEmpty(p))
                .Distinct()
                .Select(s => new TemplateDependencyInfo());
            return infos.Union(customOne).ToArray();
        }

        private static IEnumerable<string> GetTemplateDependencyNames(IClass targetClass)
        {
            return targetClass.Stereotypes
                        .Where(p => p.Name == "Template Dependency")
                        .Select(s => s.Properties.FirstOrDefault(p => p.Key == "Template Name")?.Value)
                        .ToArray();
        }

        public class TemplateDependencyInfo
        {
            public TemplateDependencyInfo()
            {
                IsCustom = true;
            }

            public TemplateDependencyInfo(string templateName, string templateId, string templateModel, string instanceType)
            {
                IsCustom = false;
                TemplateName = templateName;
                TemplateId = templateId;
                TemplateModel = templateModel;
                InstanceType = instanceType;
            }

            public bool IsCustom { get; }
            public string TemplateName { get; }
            public string TemplateId { get; }
            public string TemplateModel { get; }
            public string InstanceType { get; }
        }
    }
}
