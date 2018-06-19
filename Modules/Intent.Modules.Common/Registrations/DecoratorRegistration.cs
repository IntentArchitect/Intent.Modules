using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Engine;

namespace Intent.SoftwareFactory.Registrations
{
    public abstract class DecoratorRegistration<TDecoratorContract> : IDecoratorRegistration
    {
        public abstract string DecoratorId { get; }

        public void DoRegistration(Action<string, Type, object> register, IApplication application)
        {
            var config = application.Config.GetConfig(this.DecoratorId, Configuration.PluginConfigType.Decorator);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Decorator : { DecoratorId }.");
                return;
            }

            register(DecoratorId, typeof(TDecoratorContract), CreateDecoratorInstance(application));
        }

        public abstract object CreateDecoratorInstance(IApplication application);
    }
}
