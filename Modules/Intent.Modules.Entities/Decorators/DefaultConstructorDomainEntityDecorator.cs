using Intent.MetaModel.Domain;
using System;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class DefaultConstructorDomainEntityDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Entities.DefaultConstructorDomainEntityDecorator";

        public override string Constructors(IClass @class)
        {
            return $@"        public {@class.Name}()
        {{
        }}";
        }
    }
}
