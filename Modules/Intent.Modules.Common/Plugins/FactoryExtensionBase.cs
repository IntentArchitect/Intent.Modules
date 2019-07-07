using System.Collections.Generic;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Plugins;

namespace Intent.Modules.Common.Plugins
{
    public abstract class FactoryExtensionBase : IFactoryExtension, ISupportsConfiguration
    {
        public abstract string Id { get; }

        public virtual int Order
        {
            get; set;
        }

        public virtual void Configure(IDictionary<string, string> settings)
        {
            if (settings.ContainsKey(nameof(Order)) && !string.IsNullOrWhiteSpace(settings[nameof(Order)]))
            {
                Order = int.Parse(settings[nameof(Order)]);
            }
        }
    }
}
