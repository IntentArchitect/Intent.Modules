using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common
{
    public static class IApplicationExtensions
    {
        public static ITemplate FindTemplateInstance(this IApplication application, string templateId, object model)
        {
            return FindTemplateInstance(application, TemplateDependency.OnModel(templateId, model));
        }

        //public static ITemplate FindTemplateInstance(this IApplication application, string templateId, string className)
        //{
        //    return application.FindTemplateInstance(templateId, TemplateDependency.OnClassName(templateId, className));
        //}

        public static ITemplate FindTemplateInstance(this IApplication application, ITemplateDependency templateDependency)
        {
            return application.FindTemplateInstance(templateDependency.TemplateIdOrName, templateDependency.IsMatch);
        }

        //Typed Overloads
        public static TTemplate FindTemplateInstance<TTemplate>(this IApplication application, string templateId, string className) where TTemplate : class
        {
            return application.FindTemplateInstance(templateId, className) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IApplication application, string templateId, object model) where TTemplate : class
        {
            return application.FindTemplateInstance(templateId, model) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IApplication application, string templateId) where TTemplate : class
        {
            return application.FindTemplateInstance(TemplateDependency.OnTemplate(templateId)) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IApplication application, ITemplateDependency templateDependency) where TTemplate : class
        {
            return application.FindTemplateInstance(templateDependency.TemplateIdOrName, templateDependency.IsMatch) as TTemplate;
        }

        public static ITemplateExecutionContext FindOutputContextWithTemplateInstance(this IApplication application, string templateId, object model)
        {
            return FindOutputContextWithTemplateInstance(application, TemplateDependency.OnModel(templateId, model));
        }

        //public static ITemplateExecutionContext FindProjectWithTemplateInstance(this IApplication application, string templateId, string className)
        //{
        //    return application.FindProjectWithTemplateInstance(templateId, TemplateDependency.OnClassName(templateId, className));
        //}

        public static ITemplateExecutionContext FindOutputContextWithTemplateInstance(this IApplication application, ITemplateDependency templateDependency)
        {
            return application.FindOutputContextWithTemplateInstance(templateDependency.TemplateIdOrName, templateDependency.IsMatch);
        }
    }
}
