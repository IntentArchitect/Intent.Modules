using System.Collections;
using System.Collections.Generic;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup
{
    public abstract class CoreWebStartupDecorator : ITemplateDecorator, IPriorityDecorator
    {
        public virtual string Configuration() => @"";
        public virtual string Methods() => @"";

        public virtual int Priority { get; set; } = 0;
    }

}