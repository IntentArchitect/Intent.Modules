using Intent.SoftwareFactory.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
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
