using Intent.Modelers.Domain.Api;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using System.Collections.Generic;
using Intent.Modules.Common;

namespace Intent.Modules.Entities.Decorators
{
    public class SerializableEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        public const string Identifier = "Intent.Serializable.Entity.Interfaces.Decorator";

        public SerializableEntityInterfaceDecorator(DomainEntityInterfaceTemplate template) : base(template)
        {
        }

        public override IEnumerable<string> GetInterfaces(ClassModel @class)
        {
            var baseClass = @class.GetStereotypeProperty<string>("Serializable", "BaseType");
            if (baseClass != null)
            {
                return new[]
                {
                    $"I{baseClass}",
                };
            }

            return base.GetInterfaces(@class);
        }
    }
}