using System.Collections.Generic;
using Intent.Plugins;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class DecoratorBase : ITemplateDecorator, ISupportsConfiguration
    {
        private const string PRIORITY = "Priority";

        public virtual void Configure(IDictionary<string, string> settings)
        {
            if (this is IPriorityDecorator)
            {
                if (settings.ContainsKey(PRIORITY) && !string.IsNullOrWhiteSpace(settings[PRIORITY]))
                {
                    ((IPriorityDecorator)this).Priority = int.Parse(settings[PRIORITY]);
                }
            }
        }
    }
}
