using Intent.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Templates.OwinWebApiConfig
{
    public abstract class WebApiConfigTemplateDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<string> Configure() => new string[0];
        public virtual string Methods() => "";
        public int Priority { get; } = 0;
    }
}