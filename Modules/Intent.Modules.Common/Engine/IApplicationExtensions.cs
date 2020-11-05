using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common
{
    public interface ITemplateDependency
    {
        string TemplateId { get; }
        bool IsMatch(ITemplate template);
    }

    public interface IHasTemplateDependencies
    {
        IEnumerable<ITemplateDependency> GetTemplateDependencies();
    }

    public static class IApplicationExtensions
    {
        public static ITemplate FindTemplateInstance(this ISoftwareFactoryExecutionContext executionContext, string templateId, object model)
        {
            return FindTemplateInstance(executionContext, TemplateDependency.OnModel(templateId, model));
        }

        //public static ITemplate FindTemplateInstance(this IApplication executionContext, string templateId, string className)
        //{
        //    return executionContext.FindTemplateInstance(templateId, TemplateDependency.OnClassName(templateId, className));
        //}

        public static ITemplate FindTemplateInstance(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
        {
            return executionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch);
        }

        //Typed Overloads
        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateId, string className) where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateId, className) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateId, object model) where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateId, model) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateId) where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(TemplateDependency.OnTemplate(templateId)) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency) where TTemplate : class
        {
            return (TTemplate)executionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch);
        }

        public static IOutputTarget FindOutputTargetWithTemplate(this ISoftwareFactoryExecutionContext executionContext, string templateId, IMetadataModel hasModel)
        {
            return FindOutputTargetWithTemplate(executionContext, TemplateDependency.OnModel(templateId, hasModel));
        }

        public static IOutputTarget FindOutputTargetWithTemplate(this ISoftwareFactoryExecutionContext executionContext, ITemplateDependency templateDependency)
        {
            return executionContext.FindOutputTargetWithTemplate(templateDependency.TemplateId, templateDependency.IsMatch);
        }
    }
}
