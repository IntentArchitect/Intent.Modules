using System;
using System.IO;
using System.Text.RegularExpressions;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.ModuleBuilder.Helpers
{
    public static class TemplateHelper
    {
        /// <summary>
        /// Use <see cref="IntentTemplateBase.TryGetExistingFileContent"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string GetExistingTemplateContent<T>(IntentFileTemplateBase<T> template)
        {
            var fileLocation = template.GetExistingFilePath();

            if (fileLocation != null)
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

        //    public static IReadOnlyCollection<TemplateDependencyInfo> GetTemplateDependencyInfos(IFileTemplate template, IModuleBuilderElement classRepresentingTemplate, IEnumerable<IModuleBuilderElement> otherTemplateClasses)
        //    {
        //        var infos = GetTemplateDependencyNames(classRepresentingTemplate)
        //            .Where(p => !string.IsNullOrEmpty(p))
        //            .SelectMany(s => otherTemplateClasses.Where(p => p.Name == s))
        //            .Select(s =>
        //            {
        //                if (s.Type == ModuleBuilderElementType.CSharpTemplate)
        //                {
        //                    var templateInstance = template.Project.FindTemplateInstance<RoslynFileTemplatePartialTemplate>(RoslynFileTemplatePartialTemplate.TemplateId, s);
        //                    return new TemplateDependencyInfo(s.Name, $"{templateInstance.NormalizeNamespace(templateInstance.FullTypeName())}.TemplateId", templateInstance.Model.GetModelTypeName(), "IHasClassDetails");
        //                }
        //                else if (s.Type == ModuleBuilderElementType.FileTemplate)
        //                {
        //                    var templateInstance = template.Project.FindTemplateInstance<FileTemplatePartialTemplate>(FileTemplatePartialTemplate.TemplateId, s);
        //                    return new TemplateDependencyInfo(s.Name, $"{templateInstance.NormalizeNamespace(templateInstance.FullTypeName())}.TemplateId", templateInstance.Model.GetModelTypeName(), $"IntentFileTemplateBase<{templateInstance.Model.GetModelTypeName()}>");
        //                }
        //                return null;
        //            })
        //            .Where(p => p != null);
        //        var customOne = GetTemplateDependencyNames(classRepresentingTemplate)
        //            .Where(string.IsNullOrEmpty)
        //            .Distinct()
        //            .Select(s => new TemplateDependencyInfo());
        //        return infos.Union(customOne).ToArray();
        //    }

        //    public static IReadOnlyCollection<ITemplateDependency> GetTemplateDependencies(IModuleBuilderElement classRepresentingTemplate, IEnumerable<IModuleBuilderElement> otherTemplateClasses)
        //    {
        //        var infos = GetTemplateDependencyNames(classRepresentingTemplate)
        //            .Where(p => !string.IsNullOrEmpty(p))
        //            .SelectMany(s => otherTemplateClasses.Where(p => p.Name == s))
        //            .Select(s => 
        //            {
        //                if (s.Type == ModuleBuilderElementType.CSharpTemplate)
        //                {
        //                    return TemplateDependency.OnModel(RoslynFileTemplatePartialTemplate.TemplateId, s);
        //                }
        //                else if (s.Type == ModuleBuilderElementType.FileTemplate)
        //                {
        //                    return TemplateDependency.OnModel(FileTemplatePartialTemplate.TemplateId, s);
        //                }
        //                return null;
        //            })
        //            .Where(p => p != null)
        //            .ToArray();
        //        return infos;
        //    }

        //    private static IEnumerable<string> GetTemplateDependencyNames(IModuleBuilderElement targetClass)
        //    {
        //        return targetClass.Stereotypes
        //                    .Where(p => p.Name == "Template Dependency")
        //                    .Select(s => s.Properties.FirstOrDefault(p => p.Key == "Template Name")?.Value)
        //                    .ToArray();
        //    }

        //    public class TemplateDependencyInfo
        //    {
        //        public TemplateDependencyInfo()
        //        {
        //            IsCustom = true;
        //        }

        //        public TemplateDependencyInfo(string templateName, string templateId, string templateModel, string instanceType)
        //        {
        //            IsCustom = false;
        //            TemplateName = templateName;
        //            TemplateId = templateId;
        //            TemplateModel = templateModel;
        //            InstanceType = instanceType;
        //        }

        //        public bool IsCustom { get; }
        //        public string TemplateName { get; }
        //        public string TemplateId { get; }
        //        public string TemplateModel { get; }
        //        public string InstanceType { get; }
        //    }
    }
}
