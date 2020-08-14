using System.Collections.Generic;
using Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter;
using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class ExceptionHandlerFilterWebApiConfigDecorator : WebApiConfigTemplateDecoratorBase, IHasTemplateDependencies
    {
        public const string DecoratorId = "Intent.AspNet.WebApi.WebApiConfig.ExceptionHandlerFilter.Decorator";

        public override IEnumerable<string> Configure()
        {
            return new []{ "config.Filters.Add(new ExceptionHandlerFilter());" };
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[] { TemplateDependency.OnTemplate(WebApiFilterTemplate.TemplateId) };
        }
    }
}
