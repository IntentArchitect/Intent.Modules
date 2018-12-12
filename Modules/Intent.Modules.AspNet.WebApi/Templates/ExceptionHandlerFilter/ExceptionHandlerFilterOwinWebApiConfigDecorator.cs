using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter
{
    public class ExceptionHandlerFilterOwinWebApiConfigDecorator : WebApiConfigTemplateDecoratorBase, IHasTemplateDependencies
    {
        public const string DecoratorId = "Intent.AspNet.WebApi.ExceptionHandlerFilterOwinWebApiConfigDecorator";

        public override IEnumerable<string> Configure()
        {
            return new []{ "config.Filters.Add(new ExceptionHandlerFilter());" };
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[] { TemplateDependancy.OnTemplate(WebApiFilterTemplate.TemplateId) };
        }
    }
}
