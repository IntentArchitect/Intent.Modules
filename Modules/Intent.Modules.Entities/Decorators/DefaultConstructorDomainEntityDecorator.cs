using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntity;
using System;

namespace Intent.Modules.Entities.Decorators
{
    public class DefaultConstructorDomainEntityDecorator : DomainEntityDecoratorBase
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
