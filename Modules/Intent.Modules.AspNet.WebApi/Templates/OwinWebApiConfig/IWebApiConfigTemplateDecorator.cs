using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig
{
    public interface IWebApiConfigTemplateDecorator : ITemplateDecorator
    {
        IEnumerable<string> Configure();
    }
}