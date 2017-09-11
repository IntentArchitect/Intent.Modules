using System.Collections.Generic;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.WebApi.Templates.OwinWebApiConfig
{
    public interface IWebApiConfigTemplateDecorator : ITemplateDecorator
    {
        IEnumerable<string> Configure();
    }
}