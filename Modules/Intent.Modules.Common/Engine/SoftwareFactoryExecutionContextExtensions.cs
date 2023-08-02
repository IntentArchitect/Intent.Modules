using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;

// ReSharper disable CheckNamespace
namespace Intent.Modules.Common
{
    /// <summary>
    /// Extension methods for <see cref="ISoftwareFactoryExecutionContext"/>.
    /// </summary>
    public static class SoftwareFactoryExecutionContextExtensions
    {
        /// <summary>
        /// Finds the <see cref="IOutputTarget"/> that is targeted by the provided <paramref name="templateDependency"/>.
        /// </summary>
        public static IOutputTarget FindOutputTargetWithTemplate(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
        {
            return templateDependency is IFastLookupTemplateDependency fastLookupTemplateDependency
                ? fastLookupTemplateDependency.LookupOutputTarget(executionContext)
                : executionContext.FindOutputTargetWithTemplate(templateDependency.TemplateId, templateDependency.IsMatch);
        }

        /// <summary>
        /// Finds the <see cref="IOutputTarget" /> that is targeted by the template with the provided
        /// <paramref name="templateId" /> which is also a <see cref="ITemplateWithModel" /> and whose
        /// <see cref="ITemplateWithModel.Model" /> is a <see cref="IMetadataModel" /> whose
        /// <see cref="IMetadataModel.Id" /> matches that of the provided <paramref name="hasModel" />.
        /// </summary>
        public static IOutputTarget FindOutputTargetWithTemplate(this ISoftwareFactoryExecutionContext executionContext, string templateId, IMetadataModel hasModel)
        {
            return executionContext.FindOutputTargetWithTemplate(templateId, hasModel.Id);
        }

        /// <summary>
        /// Finds a template instance which matches the provided <paramref name="templateDependency"/>.<br/>
        /// <br/>
        /// If more than once instance is found an exception is thrown.
        /// </summary>
        public static ITemplate FindTemplateInstance(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
        {
            return templateDependency is IFastLookupTemplateDependency fastLookupTemplateDependency
                ? fastLookupTemplateDependency.LookupTemplateInstance(executionContext)
                : executionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch);
        }

        /// <summary>
        /// Finds a template instance which matches the provided <paramref name="templateDependency"/>
        /// and casts the result to <typeparamref name="TTemplate"/>.<br/>
        /// <br/>
        /// If more than once instance is found an exception is thrown.
        /// </summary>
        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency) where TTemplate : ITemplate
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateDependency);
        }

        /// <summary>
        /// Finds the template with <see cref="ITemplate.Id" /> of <paramref name="templateIdOrRole" />
        /// and casts the result to <typeparamref name="TTemplate"/>.
        /// </summary>
        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateIdOrRole) where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateIdOrRole);
        }

        /// <summary>
        /// Finds the template with <see cref="ITemplate.Id" /> of <paramref name="templateIdOrRole" />
        /// which is also a <see cref="ITemplateWithModel" /> whose
        /// <see cref="ITemplateWithModel.Model" />'s reference matches that of the provided
        /// <paramref name="model" /> and casts the result to <typeparamref name="TTemplate"/>.
        /// </summary>
        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateIdOrRole, object model) where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateIdOrRole, model);
        }

        /// <summary>
        /// Finds a template instance which has the provided <paramref name="templateIdOrRole"/> and
        /// <paramref name="modelId"/> and casts the result to <typeparamref name="TTemplate"/>.
        /// </summary>
        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateIdOrRole, string modelId)
            where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateIdOrRole, modelId);
        }

        /// <summary>
        /// Finds all template instances which match the provided <paramref name="templateDependency"/>
        /// and casts the results to <typeparamref name="TTemplate"/>.
        /// </summary>
        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
            where TTemplate : class
        {
            var templateInstances = templateDependency is IFastLookupTemplateDependency fastLookupTemplateDependency
                ? fastLookupTemplateDependency.LookupTemplateInstances(executionContext)
                : executionContext.FindTemplateInstances(templateDependency.TemplateId, templateDependency.IsMatch);

            return templateInstances.Cast<TTemplate>();
        }


        /// <summary>
        /// Finds all template instances which match the provided <paramref name="templateIdOrRole"/>
        /// and casts the results to <typeparamref name="TTemplate"/>.
        /// </summary>
        public static IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateIdOrRole)
            where TTemplate : class
        {
            return executionContext.FindTemplateInstances<TTemplate>(TemplateDependency.OnTemplate(templateIdOrRole));
        }
    }
}