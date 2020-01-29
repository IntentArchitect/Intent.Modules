using Intent.Templates;

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    public abstract class CoreWebStartupDecorator : ITemplateDecorator
    {
        public virtual string Configuration() => @"";
        public virtual string Methods() => @"";

        public virtual int Priority { get; set; } = 0;
    }
}