using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common
{
    public static class IProjectExtensions
    {
        public static string ApplicationName(this IProject project)
        {
            return project.Application.Name;
        }

        public static bool HasTemplateInstance(this IProject project, string templateId)
        {
            return project.FindTemplateInstance(templateId, (t) => true, SearchOption.OnlyThisProject) != null;
        }

        //public static ITemplate FindTemplateInstance(this IProject project, string templateId, string className)
        //{
        //    return project.FindTemplateInstance(templateId, TemplateDependency.OnClassName(templateId, className));
        //}

        public static ITemplate FindTemplateInstance(this IProject project, string templateId, IMetadataModel model)
        {
            return project.FindTemplateInstance(TemplateDependency.OnModel(templateId, model));
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IProject project, string templateId, IMetadataModel model) where TTemplate : class
        {
            return project.FindTemplateInstance(templateId, model) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IProject project, string templateId) where TTemplate : class
        {
            return project.FindTemplateInstance(templateId) as TTemplate;
        }

        public static ITemplate FindTemplateInstance(this IProject project, ITemplateDependency templateDependency)
        {
            return project.FindTemplateInstance(templateDependency.TemplateIdOrName, templateDependency.IsMatch);
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IProject project, ITemplateDependency templateDependency) where TTemplate : class
        {
            return project.FindTemplateInstance(templateDependency.TemplateIdOrName, templateDependency.IsMatch) as TTemplate;
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IProject project, ITemplateDependency templateDependency) where TTemplate : class
        {
            return project.FindTemplateInstances(templateDependency.TemplateIdOrName, templateDependency.IsMatch, SearchOption.AllProjects).Cast<TTemplate>();
        }
    }
}
