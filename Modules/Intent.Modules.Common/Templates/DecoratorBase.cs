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
            if (settings.ContainsKey(PRIORITY) && !string.IsNullOrWhiteSpace(settings[PRIORITY]))
            {
                this.Priority = int.Parse(settings[PRIORITY]);
            }
        }

        public int Priority { get; set; } = 0;
    }
}
