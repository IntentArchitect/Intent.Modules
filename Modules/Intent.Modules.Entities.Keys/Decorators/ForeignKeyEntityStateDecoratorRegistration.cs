using System.ComponentModel;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(ForeignKeyEntityStateDecorator.Identifier)]
    public class ForeignKeyEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => ForeignKeyEntityStateDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new ForeignKeyEntityStateDecorator();
        }
    }
}
