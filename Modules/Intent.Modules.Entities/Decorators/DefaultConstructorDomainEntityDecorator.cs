using Intent.Modelers.Domain.Api;
using System;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class DefaultConstructorDomainEntityDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Entities.DefaultConstructorDomainEntityDecorator";

        public DefaultConstructorDomainEntityDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

        public override string Constructors(ClassModel @class)
        {
            return $@"        public {@class.Name}()
        {{
        }}";
        }
    }
}
