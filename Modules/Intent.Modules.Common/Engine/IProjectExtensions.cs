using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Common
{
    public static class IProjectExtensions
    {
        public static string ApplicationName(this IProject project)
        {
            return project.Application.ApplicationName;
        }

        public static bool HasTemplateInstance(this IProject project, string templateId)
        {
            return project.FindTemplateInstance(templateId, (t) => true, SearchOption.OnlyThisProject) != null;
        }

        public static ITemplate FindTemplateInstance(this IProject project, string templateId, string className)
        {
            return project.FindTemplateInstance(templateId, TemplateDependancy.OnClassName(templateId, className));
        }

        public static ITemplate FindTemplateInstance(this IProject project, string templateId, object model)
        {
            return project.FindTemplateInstance(TemplateDependancy.OnModel(templateId, model));
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IProject project, string templateId, object model) where TTemplate : class
        {
            return project.FindTemplateInstance(templateId, model) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IProject project, string templateId) where TTemplate : class
        {
            return project.FindTemplateInstance(templateId) as TTemplate;
        }

        public static ITemplate FindTemplateInstance(this IProject project, ITemplateDependancy templateDependancy)
        {
            return project.FindTemplateInstance(templateDependancy.TemplateIdOrName, templateDependancy.IsMatch);
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IProject project, ITemplateDependancy templateDependancy) where TTemplate : class
        {
            return project.FindTemplateInstance(templateDependancy.TemplateIdOrName, templateDependancy.IsMatch) as TTemplate;
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IProject project, ITemplateDependancy templateDependancy) where TTemplate : class
        {
            return project.FindTemplateInstances(templateDependancy.TemplateIdOrName, templateDependancy.IsMatch, SearchOption.AllProjects).Cast<TTemplate>();
        }
    }
}
