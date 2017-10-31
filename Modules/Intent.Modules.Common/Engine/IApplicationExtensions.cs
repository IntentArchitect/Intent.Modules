using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Engine
{
    public static class IApplicationExtensions
    {
        public static ITemplate FindTemplateInstance(this IApplication application, string templateId, object model)
        {
            return FindTemplateInstance(application, TemplateDependancy.OnModel(templateId, model));
        }

        public static ITemplate FindTemplateInstance(this IApplication application, string templateId, string className)
        {
            return application.FindTemplateInstance(templateId, TemplateDependancy.OnClassName(templateId, className));
        }

        public static ITemplate FindTemplateInstance(this IApplication application, ITemplateDependancy templateDependancy)
        {
            return application.FindTemplateInstance(templateDependancy.TemplateIdOrName, templateDependancy.IsMatch);
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
            return application.FindTemplateInstance(TemplateDependancy.OnTemplate(templateId)) as TTemplate;
        }

        public static TTemplate FindTemplateInstance<TTemplate>(this IApplication application, ITemplateDependancy templateDependancy) where TTemplate : class
        {
            return application.FindTemplateInstance(templateDependancy.TemplateIdOrName, templateDependancy.IsMatch) as TTemplate;
        }

        public static IProject FindProjectWithTemplateInstance(this IApplication application, string templateId, object model)
        {
            return FindProjectWithTemplateInstance(application, TemplateDependancy.OnModel(templateId, model));
        }

        public static IProject FindProjectWithTemplateInstance(this IApplication application, string templateId, string className)
        {
            return application.FindProjectWithTemplateInstance(templateId, TemplateDependancy.OnClassName(templateId, className));
        }

        public static IProject FindProjectWithTemplateInstance(this IApplication application, ITemplateDependancy templateDependancy)
        {
            return application.FindProjectWithTemplateInstance(templateDependancy.TemplateIdOrName, templateDependancy.IsMatch);
        }
    }
}
