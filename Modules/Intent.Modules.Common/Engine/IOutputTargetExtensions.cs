#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE0130
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

        public static TTemplate? FindTemplateInstance<TTemplate>(this IOutputTarget outputTarget, string templateId, IMetadataModel model) where TTemplate : class
        {
            return FindTemplateInstance<TTemplate>(outputTarget, templateId, model, accessibleTo: outputTarget);
        }

        public static TTemplate? FindTemplateInstance<TTemplate>(this IOutputTarget outputTarget, string templateId, IMetadataModel model, IOutputTarget? accessibleTo) where TTemplate : class
        {
            return outputTarget.FindTemplateInstance(templateId, model, accessibleTo) as TTemplate;
        }

        public static ITemplate? FindTemplateInstance(this IOutputTarget outputTarget, string templateId, IMetadataModel model)
        {
            return FindTemplateInstance(outputTarget, templateId, model, accessibleTo: outputTarget);
        }

        public static ITemplate? FindTemplateInstance(this IOutputTarget outputTarget, string templateId, IMetadataModel model, IOutputTarget? accessibleTo)
        {
            return outputTarget.ExecutionContext.FindTemplateInstance(templateId, model.Id, accessibleTo);
        }

        public static ITemplate? FindTemplateInstance(this IOutputTarget outputTarget, string templateId)
        {
            return FindTemplateInstance(outputTarget, templateId, accessibleTo: outputTarget);
        }

        public static ITemplate? FindTemplateInstance(this IOutputTarget outputTarget, string templateId, IOutputTarget? accessibleTo)
        {
            return outputTarget.ExecutionContext.FindTemplateInstance(templateId, accessibleTo);
        }

        public static TTemplate? FindTemplateInstance<TTemplate>(this IOutputTarget outputTarget, string templateId) where TTemplate : class
        {
            return FindTemplateInstance<TTemplate>(outputTarget, templateId, accessibleTo: outputTarget);
        }

        public static TTemplate? FindTemplateInstance<TTemplate>(this IOutputTarget outputTarget, string templateId, IOutputTarget? accessibleTo) where TTemplate : class
        {
            return outputTarget.ExecutionContext.FindTemplateInstance(templateId, accessibleTo) as TTemplate;
        }


        public static ITemplate? FindTemplateInstance(this IOutputTarget outputTarget, ITemplateDependency templateDependency)
        {
            return FindTemplateInstance(outputTarget, templateDependency, accessibleTo: outputTarget);
        }

        public static ITemplate? FindTemplateInstance(this IOutputTarget outputTarget, ITemplateDependency templateDependency, IOutputTarget? accessibleTo)
        {
            if (templateDependency.TryGetWithAccessibleTo(accessibleTo, out var withAccessibleTo))
            {
                templateDependency = withAccessibleTo;
            }

            return outputTarget.ExecutionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch);
        }

        public static TTemplate? FindTemplateInstance<TTemplate>(this IOutputTarget outputTarget, ITemplateDependency templateDependency) where TTemplate : class
        {
            return FindTemplateInstance<TTemplate>(outputTarget, templateDependency, outputTarget) as TTemplate;
        }

        public static TTemplate? FindTemplateInstance<TTemplate>(this IOutputTarget outputTarget, ITemplateDependency templateDependency, IOutputTarget? accessibleTo) where TTemplate : class
        {
            if (templateDependency.TryGetWithAccessibleTo(accessibleTo, out var withAccessibleTo))
            {
                templateDependency = withAccessibleTo;
            }

            return outputTarget.ExecutionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch) as TTemplate;
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IOutputTarget outputTarget, ITemplateDependency templateDependency) where TTemplate : class
        {
            return FindTemplateInstances<TTemplate>(outputTarget, templateDependency, accessibleTo: outputTarget);
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IOutputTarget outputTarget, ITemplateDependency templateDependency, IOutputTarget? accessibleTo) where TTemplate : class
        {
            if (templateDependency.TryGetWithAccessibleTo(accessibleTo, out var withAccessibleTo))
            {
                templateDependency = withAccessibleTo;
            }

            return outputTarget.ExecutionContext.FindTemplateInstances(templateDependency.TemplateId, templateDependency.IsMatch).Cast<TTemplate>();
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IOutputTarget outputTarget, string templateIdOrRole) where TTemplate : class
        {
            return FindTemplateInstances<TTemplate>(outputTarget, templateIdOrRole, accessibleTo: outputTarget);
        }

        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this IOutputTarget outputTarget, string templateIdOrRole, IOutputTarget? accessibleTo) where TTemplate : class
        {
            return outputTarget.ExecutionContext.FindTemplateInstances(templateIdOrRole, accessibleTo).Cast<TTemplate>();
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstances{TTemplate}(ISoftwareFactoryExecutionContext,ITemplateDependency)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(ISoftwareFactoryExecutionContext context, ITemplateDependency templateDependency) where TTemplate : class
        {
            return context.FindTemplateInstances(templateDependency.TemplateId, templateDependency.IsMatch).Cast<TTemplate>();
        }
    }
}
