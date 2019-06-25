using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Convention.ServiceImplementations.Decorators
{
    [Description(ConventionDecorator.Identifier)]
    public class ConventionDecoratorRegistration : DecoratorRegistration<ServiceImplementationDecoratorBase>
    {
        public override string DecoratorId => ConventionDecorator.Identifier;

        private readonly IMetadataManager _metadataManager;

        public ConventionDecoratorRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ServiceImplementationDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new ConventionDecorator(_metadataManager, application);
        }
    }
}
