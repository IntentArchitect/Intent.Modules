using System;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Common.Registrations
{
    public abstract class DecoratorRegistration<TDecoratorContract> : IDecoratorRegistration
    {
        public abstract string DecoratorId { get; }

        public void DoRegistration(Action<string, Type, object> register, IApplication application)
        {
            var config = application.Config.GetConfig(this.DecoratorId, SoftwareFactory.Configuration.PluginConfigType.Decorator);
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
