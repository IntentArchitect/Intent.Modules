using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common
{
    public static class IOutputTargetExtensions
    {
        public static string ApplicationName(this IOutputTarget outputTarget)
        {
            return outputTarget.Application.Name;
        }

        //public static bool HasTemplateInstance(this IOutputTarget project, string templateId)
        //{
        //    return project.FindTemplateInstance(templateId, (t) => true, SearchOption.OnlyThisProject) != null;
        //}

        //public static ITemplate FindTemplateInstance(this IOutputTarget project, string templateId, string className)
        //{
        //    return project.FindTemplateInstance(templateId, TemplateDependency.OnClassName(templateId, className));
        //}

        public static TTemplate FindTemplateInstance<TTemplate>(this IOutputTarget project, string templateId, IMetadataModel model) where TTemplate : class
        {
            return project.FindTemplateInstance(templateId, model) as TTemplate;
        }

        public static ITemplate FindTemplateInstance(this IOutputTarget project, string templateId, IMetadataModel model)
        {
            return project.FindTemplateInstance(TemplateDependency.OnModel(templateId, model));
        }

        public static ITemplate FindTemplateInstance(this IOutputTarget project, string templateId)
        {
            return project.ExecutionContext.FindTemplateInstance(templateId);
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IOutputTarget project, string templateId) where TTemplate : class
        {
            return project.ExecutionContext.FindTemplateInstance(templateId) as TTemplate;
        }

        public static ITemplate FindTemplateInstance(this IOutputTarget project, ITemplateDependency templateDependency)
        {
            return project.ExecutionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch);
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IOutputTarget project, ITemplateDependency templateDependency) where TTemplate : class
        {
            return project.ExecutionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch) as TTemplate;
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IOutputTarget project, ITemplateDependency templateDependency) where TTemplate : class
        {
            return project.ExecutionContext.FindTemplateInstances(templateDependency.TemplateId, templateDependency.IsMatch).Cast<TTemplate>();
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this ISoftwareFactoryExecutionContext context, ITemplateDependency templateDependency) where TTemplate : class
        {
            return context.FindTemplateInstances(templateDependency.TemplateId, templateDependency.IsMatch).Cast<TTemplate>();
        }
    }
}
