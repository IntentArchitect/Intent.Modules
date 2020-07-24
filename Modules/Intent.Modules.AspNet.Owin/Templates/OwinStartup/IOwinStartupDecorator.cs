using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;

namespace Intent.Modules.AspNet.Owin.Templates.OwinStartup
{
    public interface IOwinStartupDecorator : ITemplateDecorator, IDeclareUsings
    {
        IEnumerable<string> Configuration();
        IEnumerable<string> Methods();
    }
}