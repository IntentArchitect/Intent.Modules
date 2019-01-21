using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using System;
using System.ComponentModel;

namespace Intent.Modules.Convention.ServiceImplementations.Decorators
{
    [Description(ConventionDecorator.Identifier)]
    public class ConventionDecoratorRegistration : DecoratorRegistration<ConventionDecorator>
    {
        public override string DecoratorId => ConventionDecorator.Identifier;

        private readonly IMetaDataManager _metaDataManager;

        public ConventionDecoratorRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new ConventionDecorator(_metaDataManager, application);
        }
    }
}
