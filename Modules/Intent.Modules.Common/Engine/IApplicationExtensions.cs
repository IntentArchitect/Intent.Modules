#pragma warning disable IDE0130
using System;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common
{
    /// <summary>
    /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static class IApplicationExtensions
    {
        /// <summary>
        /// Obsolete. Use <see cref="ISoftwareFactoryExecutionContext.FindTemplateInstance(string,object)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static ITemplate FindTemplateInstance(ISoftwareFactoryExecutionContext executionContext, string templateId, object model)
        {
            return executionContext.FindTemplateInstance(templateId, model);
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstance(ISoftwareFactoryExecutionContext,ITemplateDependency)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static ITemplate FindTemplateInstance(ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
        {
            return executionContext.FindTemplateInstance(templateDependency);
        }

        //Typed Overloads

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstance{TTemplate}(ISoftwareFactoryExecutionContext,string,string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static TTemplate FindTemplateInstance<TTemplate>(ISoftwareFactoryExecutionContext executionContext, string templateId, string className)
            where TTemplate : class
        {
            return executionContext.FindTemplateInstance<TTemplate>(templateId, className);
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstance{TTemplate}(ISoftwareFactoryExecutionContext,string,object)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static TTemplate FindTemplateInstance<TTemplate>(ISoftwareFactoryExecutionContext executionContext, string templateId, object model)
            where TTemplate : class
        {
            return executionContext.FindTemplateInstance<TTemplate>(templateId, model);
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstance{TTemplate}(ISoftwareFactoryExecutionContext,string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static TTemplate FindTemplateInstance<TTemplate>(ISoftwareFactoryExecutionContext executionContext, string templateId) where TTemplate : class
        {
            return executionContext.FindTemplateInstance<TTemplate>(templateId);
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindTemplateInstance{TTemplate}(ISoftwareFactoryExecutionContext,ITemplateDependency)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static TTemplate FindTemplateInstance<TTemplate>(ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
            where TTemplate : ITemplate
        {
            return executionContext.FindTemplateInstance<TTemplate>(templateDependency);
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindOutputTargetWithTemplate(ISoftwareFactoryExecutionContext,string,IMetadataModel)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static IOutputTarget FindOutputTargetWithTemplate(ISoftwareFactoryExecutionContext executionContext, string templateId, IMetadataModel hasModel)
        {
            return executionContext.FindOutputTargetWithTemplate(templateId, hasModel);
        }

        /// <summary>
        /// Obsolete. Use <see cref="SoftwareFactoryExecutionContextExtensions.FindOutputTargetWithTemplate(ISoftwareFactoryExecutionContext,ITemplateDependency)"/> instead.
        /// </summary>
        public static IOutputTarget FindOutputTargetWithTemplate(ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
        {
            return executionContext.FindOutputTargetWithTemplate(templateDependency);
        }
    }
}
