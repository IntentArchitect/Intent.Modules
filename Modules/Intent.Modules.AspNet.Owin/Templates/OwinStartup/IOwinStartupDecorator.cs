using Intent.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public interface IOwinStartupDecorator : ITemplateDecorator, IPriorityDecorator, IDeclareUsings
    {
        IEnumerable<string> Configuration();
        IEnumerable<string> Methods();
    }

}