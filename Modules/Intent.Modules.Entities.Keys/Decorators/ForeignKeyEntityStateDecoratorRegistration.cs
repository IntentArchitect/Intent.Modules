using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(ForeignKeyEntityStateDecorator.Identifier)]
    public class ForeignKeyEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => ForeignKeyEntityStateDecorator.Identifier;
        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new ForeignKeyEntityStateDecorator(template);
        }
    }
}
