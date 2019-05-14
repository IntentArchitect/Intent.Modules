using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Registrations
{
    public abstract class DecoratorRegistration<TTemplate, TDecoratorContract> : IDecoratorRegistration
        where TTemplate : IHasDecorators<TDecoratorContract>
        where TDecoratorContract : ITemplateDecorator
    {
        public void DoRegistration(IDecoratorRegistry registry, IApplication application)
        {
            var config = application.Config.GetConfig(this.DecoratorId, PluginConfigType.Decorator);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Decorator : { DecoratorId }.");
                return;
            }

            registry.RegisterDecorator<TTemplate, TDecoratorContract>(DecoratorId, (template) => CreateDecoratorInstance(template, application) );
        }

        public abstract string DecoratorId { get; }

        public abstract TDecoratorContract CreateDecoratorInstance(TTemplate template, IApplication application);
    }

    public abstract class DecoratorRegistration<TDecoratorContract> : DecoratorRegistration<IHasDecorators<TDecoratorContract>, TDecoratorContract>
        where TDecoratorContract : ITemplateDecorator
    {
        public override TDecoratorContract CreateDecoratorInstance(IHasDecorators<TDecoratorContract> template, IApplication application)
        {
            return CreateDecoratorInstance(application);
        }

        public abstract TDecoratorContract CreateDecoratorInstance(IApplication application);
    }
}
