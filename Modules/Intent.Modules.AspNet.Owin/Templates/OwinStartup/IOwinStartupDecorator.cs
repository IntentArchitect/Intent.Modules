using System.Collections.Generic;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Owin.Templates.OwinStartup
{
    public interface IOwinStartupDecorator : ITemplateDecorator, IPriorityDecorator, IDeclareUsings
    {
        IEnumerable<string> Configuration();
        IEnumerable<string> Methods();
    }

}