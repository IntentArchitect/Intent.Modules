using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common
{
    /// <summary>
    /// Extension methods for <see cref="IOutputTarget"/>.
    /// </summary>
    public static class IOutputTargetExtensions
    {
        public static string ApplicationName(this IOutputTarget outputTarget)
        {
            return outputTarget.Application.Name;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IOutputTarget project, string templateId, IMetadataModel model) where TTemplate : class
        {
            return project.FindTemplateInstance(templateId, model) as TTemplate;
        }

        public static ITemplate FindTemplateInstance(this IOutputTarget project, string templateId, IMetadataModel model)
        {
            return project.ExecutionContext.FindTemplateInstance(templateId, model.Id);
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

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IOutputTarget project, string templateIdOrRole) where TTemplate : class
        {
            // TODO: Use overload without predicate with SDK 3.5.0
            return project.ExecutionContext.FindTemplateInstances(templateIdOrRole, () => true).Cast<TTemplate>();
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstances{TTemplate}"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(ISoftwareFactoryExecutionContext context, ITemplateDependency templateDependency) where TTemplate : class
        {
            return context.FindTemplateInstances(templateDependency.TemplateId, templateDependency.IsMatch).Cast<TTemplate>();
        }
    }
}
